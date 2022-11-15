using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Assignment4
{
    public class Asteroid : GameObject
    {
        public Model Model;

        public bool isActive { get; set; }

        public Vector3 position;
        public Vector3 direction;
        public float speed;

        public Asteroid(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("asteroid1");
            Model = Content.Load<Model>("asteroid4");
            Renderer renderer = new Renderer(Model, Transform, camera, light, Content, graphicsDevice, 20f, texture, null, 1);
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * GameConstants.AsteroidBoundingSphereScale;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
            //*** Additional Property (for Asteroid, isActive = true)
            isActive = false;
        }

        public override void Update()
        {
            if (this.Transform.Position.X > GameConstants.PlayfieldSizeX)
            {
                this.Transform.LocalPosition -= Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX;
            }

            if (this.Transform.Position.X < -GameConstants.PlayfieldSizeX)
            {
                this.Transform.LocalPosition += Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX;
            }

            if (this.Transform.Position.Y > GameConstants.PlayfieldSizeY)
            {
                this.Transform.LocalPosition -= Vector3.UnitY * 2 * GameConstants.PlayfieldSizeY;
            }

            if (this.Transform.Position.Y < -GameConstants.PlayfieldSizeY)
            {
                this.Transform.LocalPosition += Vector3.UnitY * 2 * GameConstants.PlayfieldSizeY;
            }

            base.Update();
        }

        public void Update(float delta)
        {
            position += direction * speed * GameConstants.AsteroidSpeedAdjustment * delta;
            if (position.X > GameConstants.PlayfieldSizeX) position.X -= 2 * GameConstants.PlayfieldSizeX;
            if (position.X < -GameConstants.PlayfieldSizeX) position.X += 2 * GameConstants.PlayfieldSizeX;
            if (position.Y > GameConstants.PlayfieldSizeY) position.Y -= 2 * GameConstants.PlayfieldSizeY;
            if (position.Y < -GameConstants.PlayfieldSizeY) position.Y += 2 * GameConstants.PlayfieldSizeY;
        }

        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}
