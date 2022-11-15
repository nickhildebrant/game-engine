using System;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment4
{
    class Ship : GameObject
    {
        public Model Model;

        SoundEffect soundEngine;
        SoundEffectInstance soundInstance;

        public Ship(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("wedge_p1_diff_v1");
            Model = Content.Load<Model>("p1_wedge");
            Renderer renderer = new Renderer(Model, Transform, camera, light, Content, graphicsDevice, 20f, texture, null, 1);
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * GameConstants.ShipBoundingSphereScale;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            soundEngine = Content.Load<SoundEffect>("engine_2");
            soundInstance = soundEngine.CreateInstance();

            Transform.Rotate(Vector3.Right, MathHelper.PiOver2);
        }

        public override void Update()
        {

            // Input handling for the ship
            if(InputManager.IsKeyDown(Keys.W))
            {
                Rigidbody.Velocity += Transform.Forward * Time.ElapsedGameTime * GameConstants.PlayerSpeed;
            }

            if (InputManager.IsKeyDown(Keys.S))
            {
                Rigidbody.Velocity += Transform.Backward * Time.ElapsedGameTime * GameConstants.PlayerSpeed;
            }

            if (InputManager.IsKeyDown(Keys.A))
            {
                Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * GameConstants.PlayerRotationSpeed);
            }

            if (InputManager.IsKeyDown(Keys.D))
            {
                Transform.Rotate(Vector3.Down, Time.ElapsedGameTime * GameConstants.PlayerRotationSpeed);
            }

            // Handles when the ship goes out of the border
            if (Transform.Position.X > GameConstants.PlayfieldSizeX) { Transform.LocalPosition -= Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX; }
            if (Transform.Position.X < -GameConstants.PlayfieldSizeX) { Transform.LocalPosition += Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX; }
            if (Transform.Position.Y > GameConstants.PlayfieldSizeY) { Transform.LocalPosition -= Vector3.UnitY * 2 * GameConstants.PlayfieldSizeY; }
            if (Transform.Position.Y < -GameConstants.PlayfieldSizeY) { Transform.LocalPosition += Vector3.UnitY * 2 * GameConstants.PlayfieldSizeY; }

            // Handles sound
            if (InputManager.IsKeyDown(Keys.W) || InputManager.IsKeyDown(Keys.A) || InputManager.IsKeyDown(Keys.S) || InputManager.IsKeyDown(Keys.D))
            {
                soundInstance.Volume = 0.75f;
                soundInstance.IsLooped = true;
                soundInstance.Play();
            } 
            else soundInstance.Stop();

            base.Update();
        }
    }
}