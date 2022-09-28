﻿using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Collider
    {
        public virtual bool Collides(Collider other, out Vector3 normal)
        {
            normal = Vector3.Zero;
            return false;
        }
    }
}
