using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assignment3
{
    public class Assignment3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;

        Random random = new Random();
        int numberCollisions = 0;

        bool isShowing = true;
        int numSpheres = 1;
        float speed = 1.0f;

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
            font = Content.Load<SpriteFont>("font");

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

            gameObjects = new List<GameObject> { };

            AddGameObject();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            InputManager.Update();

            // Shift Hides info
            if (InputManager.IsKeyPressed(Keys.LeftShift) || InputManager.IsKeyPressed(Keys.RightShift)) isShowing = !isShowing;

            // LEFT/RIGHT controls speed
            if (InputManager.IsKeyDown(Keys.Right)) speed += Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Left) && speed > 0.1f) speed -= Time.ElapsedGameTime;

            // UP/DOWN add or removes spheres
            if (InputManager.IsKeyPressed(Keys.Up)) { numSpheres++; AddGameObject(); }
            if (InputManager.IsKeyPressed(Keys.Down) && numSpheres > 0) { gameObjects.RemoveAt(numSpheres-1); numSpheres--; }

            // Update each GameObject
            foreach (GameObject gameObject in gameObjects) gameObject.Update();

            Vector3 normal; // it is updated if a collision happens
            for (int i = 0; i < gameObjects.Count; i++)
            {
                // Colliding with box collider
                if (boxCollider.Collides(gameObjects[i].Collider, out normal))
                {
                    numberCollisions++;
                    Debug.WriteLine("Collision Detected: " + numberCollisions);
                    //if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0) rigidbodies[i].Impulse += Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                    if (Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) < 0) gameObjects[i].Rigidbody.Impulse += Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) * -2 * normal;
                }

                for (int j = i + 1; j < gameObjects.Count; j++)
                {
                    if (gameObjects[i].Collider.Collides(gameObjects[j].Collider, out normal)) numberCollisions++;

                    Vector3 velocityNormal = Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity - gameObjects[j].Rigidbody.Velocity) * -2 * normal * gameObjects[i].Rigidbody.Mass * gameObjects[j].Rigidbody.Mass;
                    gameObjects[i].Rigidbody.Impulse += velocityNormal / 2;
                    gameObjects[j].Rigidbody.Impulse += -velocityNormal / 2;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw();
                _spriteBatch.DrawString(font, gameObject.Transform.LocalPosition.ToString(), new Vector2(400, 10), Color.Black);
            }

            _spriteBatch.DrawString(font, "SHIFT - Show/Hide Details", new Vector2(5, 10), Color.Black);

            if(isShowing)
            {
                _spriteBatch.DrawString(font, "Arrows (LEFT/RIGHT) - Speed: " + speed.ToString("0.0"), new Vector2(5, 25), Color.Black);
                _spriteBatch.DrawString(font, "Arrows (UP/DOWN) - # of Spheres: " + numSpheres, new Vector2(5, 40), Color.Black);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void AddGameObject()
        {
            GameObject gameObject = new GameObject();

            // create new Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Mass = (float)random.NextDouble();

            // set the direction and velocity
            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 10);
            gameObject.Add<Rigidbody>(rigidbody);

            // Create new sphereCollider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f;
            sphereCollider.Transform = rigidbody.Transform;
            gameObject.Add<Collider>(sphereCollider);

            // Lab 7-E
            Renderer renderer = new Renderer(sphereModel, rigidbody.Transform, camera, light, Content, GraphicsDevice, 20f, Content.Load<Texture2D>("Square"), "SimpleShading", 1);
            gameObject.Add<Renderer>(renderer);

            // Adding the game object to the list
            gameObjects.Add(gameObject);
        }
    }
}