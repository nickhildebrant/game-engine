using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System.Threading;

namespace CPI311.Labs
{
    public class Lab7 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;

        BoxCollider boxCollider;
        SphereCollider sphere1, sphere2;

        Model model;
        Transform modelTransform;

        Camera camera;
        Transform cameraTransform;

        Light light;

        // *** Not using GameObject[]
        List<Transform> transforms;
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        List<Renderer> renderers; // lab 7 update

        bool haveThreadRunning;
        int lastSecondCollision = 0;
        int numberCollisions = 0;

        Random random;

        public Lab7()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();

            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            renderers = new List<Renderer>();

            haveThreadRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");

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

            light = new Light();
            Transform lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 10 + Vector3.Right * 5;
            light.Transform = lightTransform;

            //for (int i = 0; i < 5; i++) AddSphere();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            foreach (Rigidbody rigidbody in rigidbodies) rigidbody.Update();

            if (InputManager.IsKeyPressed(Keys.Space)) AddSphere();

            Vector3 normal; // it is updated if a collision happens
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberCollisions++;
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0) rigidbodies[i].Impulse += Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }

                for (int j = i + 1; j < transforms.Count; j++)
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

            for (int i = 0; i < transforms.Count; i++)
            {
                float speed = rigidbodies[i].Velocity.Length();
                float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = new Vector3(speedValue, speedValue, 1);
                model.Draw(transforms[i].World, camera.View, camera.Projection);
            }

            //for (int i = 0; i < renderers.Count; i++) renderers[i].Draw();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "lastSecondCollion = " + lastSecondCollision, new Vector2(5, 10), Color.Black);
            _spriteBatch.DrawString(font, "numberOfCollisions = " + numberCollisions, new Vector2(5, 25), Color.Black);
            _spriteBatch.DrawString(font, "Press SPACE to add a Sphere", new Vector2(5, 60), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // ** Lab 7 private Methods

        private void CollisionReset(Object obj) 
        {
            while(haveThreadRunning)
            {
                lastSecondCollision = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void AddSphere()
        {
            // create new Transform and Rigidbody
            Transform transform = new Transform();
            transform.LocalPosition += Vector3.Right * 10 * (float)random.NextDouble(); //avoid overlapping each sphere 
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;
            rigidbody.Mass = 1;

            // set the direction and velocity
            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);

            // Create new sphereCollider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
            sphereCollider.Transform = transform;

            // Lab 7-E
            Renderer renderer = new Renderer(model, transform, camera, light, Content, GraphicsDevice, 20f, Content.Load<Texture2D>("Square"), "SimpleShading", 1);
            renderers.Add(renderer);

            // Add each element to its respective list
            transforms.Add(transform);
            colliders.Add(sphereCollider);
            rigidbodies.Add(rigidbody);
        }
    }
}