using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Assignment4
{
    public class Asteroid : GameObject
    {
        public bool isActive { get; set; }

        public Asteroid(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("asteroid1");
            Renderer renderer = new Renderer(Content.Load<Model>("asteroid4"), Transform, camera, light, Content, graphicsDevice, 20f, texture, null, 1);
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
            //*** Additional Property (for Asteroid, isActive = true)
            isActive = false;
        }

        public override void Update()
        {
            if (!isActive) return;

            if (Transform.Position.X > GameConstants.PlayfieldSizeX ||
               Transform.Position.X < -GameConstants.PlayfieldSizeX ||
               Transform.Position.Z > GameConstants.PlayfieldSizeY ||
               Transform.Position.Z < -GameConstants.PlayfieldSizeY)
            {
                isActive = false;
                Rigidbody.Velocity = Vector3.Zero; // stop moving
            }

            base.Update();
        }

        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}
