﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System.Timers;
using System.Net.Sockets;

namespace Server
{
    /// <summary>
    /// The whole game universe
    /// </summary>
    public class Universe
    {
        public List<Entity> Entites = new List<Entity>();
        public List<Player> Players = new List<Player>();

        public World World;

        DateTime lastUpdate = DateTime.Now;

        Timer timer;

        const float interval = 30f;
        const int iterations = 10;

        /// <summary>
        /// Creates an instance of the Universe class
        /// </summary>
        public Universe()
        {
            World = new World(Vector2.Zero) { AutoClearForces = true };

            
            timer = new Timer(interval);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.AutoReset = false;

            Update();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Update();
        }

        public void AddPlayer(Player player)
        {            
            Players.Add(player);
            player.Universe = this;
            //GenerateShip(player);
        }

        public void RemovePlayer(Player player)
        {
            Entites.Remove(player.Controlling);
            Players.Remove(player);
        }

        public Ship GenerateShip(Player player)
        {
            player.Controlling = new Ship(player.Name, 10);
            player.UpdateFaction();
            AddEntity(player.Controlling, GetSpawnLocation());
            return player.Controlling;
        }

        public Vector2 GetSpawnLocation()
        {
            return new Vector2(0, -500);
        }

        public void AddEntity(Entity entity, Vector2 position)
        {
            entity.CreateBody(World);
            entity.Position = position;
            entity.Universe = this;
            entity.Fixture.Body.IsBullet = true;
            Entites.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            Entites.Remove(entity);
            entity.Fixture.Dispose();
            entity.Fixture = null;

            foreach (var player in Players)
            {
                player.AddNote(entity.Id, NoteType.EntityRemoved);
            }
        }

        void Update()
        {
            foreach (var entity in Entites.ToArray())
            {
                entity.Update();

                //GameServer.Instance.Log(string.Format("{0}={1}@({2},{3})", entity.Id, entity.Name, entity.Position.X, entity.Position.Y));
            }

            //var time = (float)(DateTime.Now - lastUpdate).TotalMilliseconds;
            for (int i = 0; i < iterations; i++)
            {
                World.Step(interval / iterations);
            }

            foreach (var player in Players.ToArray())
            {
                player.Update();

                if (player.Client != null)
                {
                    var packet = new UpdatePacket()
                        {
                            ControllingEntityId = player.Controlling == null ? 0 : player.Controlling.Id,
                            Notes = player.Notes,
                            Messages = player.Messages,
                            TractorRange = player.Controlling == null ? 0 : (int)player.Controlling.TractorQuadrance
                        };

                    //GameServer.Instance.Log(string.Format("{0}", player.Controlling.Fixture.Body.LinearVelocity));

                    foreach (var entity in Entites)
                    {
                        var type = entity.GetClientType();
                        if (type != EntityUpdateType.Invisible)
                            packet.Entities.Add(new EntityUpdate()
                                {
                                    Type = entity.GetClientType(),
                                    Id = entity.Id, LocationX = (int)(entity.Fixture.Body.Position.X * 10000),
                                    LocationY = (int)(entity.Fixture.Body.Position.Y * 10000),
                                    Rotation = (int)(entity.Fixture.Body.Rotation * 100f),
                                    Size = (int)(entity.Fixture.Shape.Radius * 100f)
                                });
                    }

                    var packetBytes = packet.Serialize();

                    player.Notes.Clear();
                    player.Messages.Clear();

                    try
                    {
                        player.Client.Client.Send(packetBytes);
                    }
                    catch (SocketException e)
                    {
                        GameServer.Instance.Log(e.ToString());
                        player.Disconnect();
                    }
                }
            }

            lastUpdate = DateTime.Now;
            timer.Start();
        }
    }
}
