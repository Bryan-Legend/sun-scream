﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace Server
{
    /// <summary>
    /// Stores information about a star system
    /// </summary>
    public class StarSystem : Entity
    {
        private const float size = 10f;
        public StarSystem(int id, string name) : base(id, name)
        {

        }

        internal override EntityUpdateType GetClientType()
        {
            return EntityUpdateType.Star;
        }

        protected override Fixture GetFixture(World world)
        {
            return FixtureFactory.CreateRectangle(world, size, size, 1);
            //return FixtureFactory.CreateCircle(world,1, new Body( Width, Height, 1);
        }
    }
}