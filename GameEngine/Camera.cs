using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; // Lab 8 Extension

namespace CPI311.GameEngine
{
    public class Camera : Component
    {
        // public Transform Transform { get; set; } conflict from component class
        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }
        public float NearPlane { get; set; }
        public float FarPlane { get; set; }

        // *** Lab 8 Properties
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Viewport Viewport
        {
            get
            {
                return new Viewport((int)(ScreenManager.Width * Position.X), (int)(ScreenManager.Height * Position.Y),
                                    (int)(ScreenManager.Width * Size.X), (int)(ScreenManager.Height * Size.Y));
            }
        }

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

        // For raycasting
        public Ray ScreenPointToWorldRay(Vector2 position)
        {
            Vector3 start = Viewport.Unproject(new Vector3(position, 0), Projection, View, Matrix.Identity);
            Vector3 end = Viewport.Unproject(new Vector3(position, 1), Projection, View, Matrix.Identity);
            return new Ray(start, end - start);
        }
    }
}
