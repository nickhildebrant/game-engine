using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System.Diagnostics;

namespace CPI311.Labs
{
    public class Lab6 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        BoxCollider boxCollider;
        SphereCollider sphere1, sphere2;

        Model model;
        Transform modelTransform;

        Camera camera;
        Transform cameraTransform;

        // *** Not using GameObject[]
        List<Transform> transforms;
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;

        int numberCollisions = 0;

        Random random;

        public Lab6()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();

            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            model = Content.Load<Model>("Sphere");
            modelTransform = new Transform();

            // **** Lighting ****
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            // ******************

            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera.Transform = cameraTransform;

            for (int i = 0; i < 2; i++)
            {
                Transform transform = new Transform();
                transform.LocalPosition += Vector3.Right * 2 * i; //avoid overlapping each sphere 
                Rigidbody rigidbody = new Rigidbody();
                rigidbody.Transform = transform;
                rigidbody.Mass = 1;

                Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                direction.Normalize();
                rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);

                SphereCollider sphereCollider = new SphereCollider();
                sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
                sphereCollider.Transform = transform;

                transforms.Add(transform);
                colliders.Add(sphereCollider);
                rigidbodies.Add(rigidbody);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            foreach (Rigidbody rigidbody in rigidbodies) rigidbody.Update();

            Vector3 normal; // it is updated if a collision happens
            for(int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberCollisions++;
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0) rigidbodies[i].Impulse += Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }

                for(int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal)) numberCollisions++;

                    Vector3 velocityNormal = Vector3.Dot(normal, rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2 * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                    rigidbodies[i].Impulse += velocityNormal / 2;
                    rigidbodies[j].Impulse += -velocityNormal / 2;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach(Transform transform in transforms)
            {
                model.Draw(transform.World, camera.View, camera.Projection);
            }

            base.Draw(gameTime);
        }
    }
}