using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System;
using System.Collections.Generic;

namespace Assignment3
{
    public class Assignment3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Random random = new Random();

        Model sphereModel;

        Camera camera;
        Light light;
        BoxCollider boxCollider;
        List<GameObject> gameObjects;

        public Assignment3()
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
            ScreenManager.Initialize(_graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            sphereModel = Content.Load<Model>("Sphere");

            camera = new Camera();
            Transform cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera.Transform = cameraTransform;

            light = new Light();
            Transform lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 10 + Vector3.Right * 5;
            light.Transform = lightTransform;

            boxCollider = new BoxCollider();
            gameObjects = new List<GameObject>();

            AddGameObject();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            InputManager.Update();

            foreach (GameObject gameObject in gameObjects) gameObject.Update();

            foreach (GameObject gameObject in gameObjects) gameObject.Draw();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void AddGameObject()
        {
            GameObject gameObject = new GameObject();

            // create new Transform and Rigidbody
            Transform transform = new Transform();
            transform.LocalPosition += Vector3.Right * 10 * (float)random.NextDouble(); //avoid overlapping each sphere 
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;
            rigidbody.Mass = (float)random.NextDouble();

            // set the direction and velocity
            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);

            // Create new sphereCollider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
            sphereCollider.Transform = transform;

            // Lab 7-E
            Renderer renderer = new Renderer(sphereModel, transform, camera, light, Content, GraphicsDevice, 20f, Content.Load<Texture2D>("Square"), "SimpleShading", 1);

            // Adding components to the GameObject
            gameObject.Add<Rigidbody>(rigidbody);
            gameObject.Add<Collider>(sphereCollider);
            gameObject.Add<Renderer>(renderer);

            // Adding the game object to the list
            gameObjects.Add(gameObject);
        }
    }
}