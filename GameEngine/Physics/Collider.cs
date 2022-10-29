using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Collider : Component
    {
        public virtual bool Collides(Collider other, out Vector3 normal)
        {
            normal = Vector3.Zero;
            return false;
        }

        // Lab 8 Extension for Raycasting
        public virtual float? Intersects(Ray ray) { return null; }
    }
}
