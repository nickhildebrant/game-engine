using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Camera
    {
        public Transform Transform { get; set; }

        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }
        public float NearPlane { get; set; }
        public float FarPlane { get; set; }

        public Matrix Projection
        {
            get { return Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane); }
        }

        public Matrix View
        {
            get { return Matrix.CreateLookAt(Transform.LocalPosition, Transform.LocalPosition + Transform.Forward, Transform.Up); }
        }

        // *** Constructor
        public Camera()
        {
            Transform = null;   // initialize in Game class
            FieldOfView = MathHelper.PiOver2;
            AspectRatio = 1.33f;
            NearPlane = 0.1f;
            FarPlane = 100f;
        }
    }
}
