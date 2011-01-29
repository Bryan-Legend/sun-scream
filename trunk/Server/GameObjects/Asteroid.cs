﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;

namespace Server
{
    /// <summary>
    /// Stores information about a ship
    /// </summary>
    public class Asteroid : Entity
    {
        private const float size = 0.1f;

        public Asteroid(int id, string name) : base(id, name)
        {
        }

        protected override Fixture GetFixture(World world)
        {
            throw new NotImplementedException();
        }

        internal override EntityUpdateType GetClientType()
        {
            return EntityUpdateType.Ship;
        }
    }
}