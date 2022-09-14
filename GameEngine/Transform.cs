using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Transform
    {
        // SRT - Scale * Rotation * Transform
        private Vector3 localScale;
        private Quaternion localRotation;
        private Vector3 localPosition;
        private Matrix world;

        public Vector3 LocalPosition
        {
            get { return localPosition; }
            set { localPosition = value; UpdateWorld(); }
        }

        public Vector3 LocalScale
        {
            get { return localScale; }
            set { localScale = value; UpdateWorld(); }
        }

        public Quaternion LocalRotation
        {
            get { return localRotation; }
            set { localRotation = value; UpdateWorld(); }
        }

        public Matrix World { get { return world; } }

        // *** Properties for directions
        public Vector3 Forward { get { return world.Forward; } }
        public Vector3 Backward { get { return world.Backward; } }
        public Vector3 Right { get { return world.Right; } }
        public Vector3 Left { get { return world.Left; } }
        public Vector3 Up { get { return world.Up; } }
        public Vector3 Down { get { return world.Down; } } 

        // *** Constructor
        public Transform()
        {
            localScale = Vector3.One;
            localRotation = Quaternion.Identity;
            localPosition = Vector3.Zero;
            UpdateWorld();
        }

        private void UpdateWorld()
        {
            world = Matrix.CreateScale(localScale) * Matrix.CreateFromQuaternion(localRotation) * Matrix.CreateTranslation(localPosition);
        }

        // *** Rotate Method
        public void Rotate(Vector3 axis, float angle)
        {
            localRotation *= Quaternion.CreateFromAxisAngle(axis, angle);
            UpdateWorld();
        }
    }
}
