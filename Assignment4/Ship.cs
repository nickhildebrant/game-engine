using System;
using System.Reflection.Metadata;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GoingBeyond4
{
    class Ship : GameObject
    {
        public Model Model;

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
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
        }

        public override void Update()
        {
            if(InputManager.IsKeyPressed(Keys.W))
            {
                Rigidbody.Velocity += Vector3.Forward * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyPressed(Keys.A))
            {
                Transform.Rotate(Transform.Up, Time.ElapsedGameTime);
            }
            if (InputManager.IsKeyPressed(Keys.S))
            {
                Transform.Rotate(Transform.Down, Time.ElapsedGameTime);
            }
            if (InputManager.IsKeyPressed(Keys.D))
            {
                Rigidbody.Velocity += Vector3.Backward * Time.ElapsedGameTime;
            }

            base.Update();
        }
    }
}