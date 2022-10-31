using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Security.Cryptography;

namespace Assignment3
{
    public class Assignment3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;

        Random random = new Random();
        
        bool isShowing = true;
        bool renderingColor = false;
        int numSpheres = 1;
        float speed = 1.0f;

        float frameTotal, averageFrames = 60;
        Queue<float> frameRates = new Queue<float>();

        int averageCollisions = 0;
        Queue<int> collisions = new Queue<int>();
        bool haveThreadRunning;
        int numberCollisions = 0;

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

            sphereModel = Content.Load<Model>("Sphere");

            // **** Lighting ****
            foreach (ModelMesh mesh in sphereModel.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            // ******************

            camera = new Camera();
            Transform cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera.Transform = cameraTransform;

            light = new Light();
            Transform lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 10 + Vector3.Right * 5;
            light.Transform = lightTransform;

            gameObjects = new List<GameObject> { };

            AddGameObject();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime, speed);
            InputManager.Update();

            if (collisions.Count == 5)
            {
                frameRates.Dequeue();
            }

            frameRates.Enqueue(numberCollisions);

            // Shift Hides info
            if (InputManager.IsKeyPressed(Keys.LeftShift) || InputManager.IsKeyPressed(Keys.RightShift)) isShowing = !isShowing;

            // LEFT/RIGHT controls speed
            if (InputManager.IsKeyDown(Keys.Right)) speed += Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Left) && speed > 0.1f) speed -= Time.ElapsedGameTime;

            // UP/DOWN add or removes spheres
            if (InputManager.IsKeyPressed(Keys.Up)) { numSpheres++; AddGameObject(); }
            if (InputManager.IsKeyPressed(Keys.Down) && numSpheres > 0) { gameObjects.RemoveAt(numSpheres-1); numSpheres--; }

            // Toggles Color rendering
            if(InputManager.IsKeyPressed(Keys.Space))
            {
                renderingColor = true;
            }

            // Toggles texure rendering
            if(InputManager.IsKeyPressed(Keys.LeftAlt) || InputManager.IsKeyPressed(Keys.RightAlt))
            {
                renderingColor = false;
            }

            // Update each GameObject
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update();
            }

            Vector3 normal; // it is updated if a collision happens
            for (int i = 0; i < gameObjects.Count; i++)
            {
                // Colliding with box collider
                if (boxCollider.Collides(gameObjects[i].Collider, out normal))
                {
                    numberCollisions++;
                    if (Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) < 0) gameObjects[i].Rigidbody.Impulse += Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) * -(2*gameObjects[i].Rigidbody.Mass) * normal;
                }

                for (int j = i + 1; j < gameObjects.Count; j++)
                {
                    if (gameObjects[i].Collider.Collides(gameObjects[j].Collider, out normal)) numberCollisions++;

                    Vector3 velocityNormal = Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity - gameObjects[j].Rigidbody.Velocity) * -2 * gameObjects[i].Rigidbody.Mass * gameObjects[j].Rigidbody.Mass * normal;
                    gameObjects[i].Rigidbody.Impulse += velocityNormal / (2 * gameObjects[j].Rigidbody.Mass);
                    gameObjects[j].Rigidbody.Impulse += -velocityNormal / (2 * gameObjects[i].Rigidbody.Mass);
                }
            }

            base.Update(gameTime);
        }

        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
  
                int totalCollisions = 0;
                foreach (int count in collisions)
                {
                    totalCollisions += count;
                }

                averageCollisions = numberCollisions / 5;
                numberCollisions = 0;

                System.Threading.Thread.Sleep(5000);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            _spriteBatch.Begin();

            if (frameRates.Count >= 10)
            {
                frameTotal -= frameRates.Peek();
                frameRates.Dequeue();
            }

            //Debug.WriteLine(1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (Time.TotalGameTime.Seconds % 1 == 0)
            {
                var newCount = 1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds;
                frameTotal += newCount;
                frameRates.Enqueue(newCount);
            }

            if (Time.TotalGameTime.Seconds % 10 == 0)
            {
                averageFrames = 1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            foreach (GameObject gameObject in gameObjects)
            {
                if (renderingColor)
                {
                    float speed = gameObject.Rigidbody.Velocity.Length();
                    float speedValue = MathHelper.Clamp(speed / 35f, 0, 1);
                    (sphereModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = new Vector3(speedValue, speedValue, speedValue);
                    sphereModel.Draw(gameObject.Transform.World, camera.View, camera.Projection);
                }
                else gameObject.Draw();
            }

            _spriteBatch.DrawString(font, "SHIFT - Show/Hide Details", new Vector2(5, 10), Color.Black);

            if(isShowing)
            {
                _spriteBatch.DrawString(font, "Arrows (LEFT/RIGHT) - Speed: " + speed.ToString("0.0"), new Vector2(5, 25), Color.Black);
                _spriteBatch.DrawString(font, "Arrows (UP/DOWN) - # of Spheres: " + numSpheres, new Vector2(5, 40), Color.Black);
                _spriteBatch.DrawString(font, "Average Frame Rate (10s): " + averageFrames.ToString("0.00"), new Vector2(5, 55), Color.Black);
                _spriteBatch.DrawString(font, "Average Collisions (5s): " + averageCollisions, new Vector2(5, 70), Color.Black);
                _spriteBatch.DrawString(font, "SPACE - Show Speed Colors", new Vector2(5, 85), Color.Black);
                _spriteBatch.DrawString(font, "ALT - Show Textures", new Vector2(5, 100), Color.Black);
                
            }
            else
            {
                _spriteBatch.DrawString(font, "SPACE - Show Speed Colors", new Vector2(5, 25), Color.Black);
                _spriteBatch.DrawString(font, "ALT - Show Textures", new Vector2(5, 40), Color.Black);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void AddGameObject()
        {
            GameObject gameObject = new GameObject();
            gameObject.Transform.LocalPosition = new Vector3((float)random.NextDouble() * boxCollider.Size, 
                                                             (float)random.NextDouble() * boxCollider.Size, 
                                                             (float)random.NextDouble() * boxCollider.Size
                                                            );

            // set the direction and velocity
            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();

            // create new Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Mass = (float)random.NextDouble()*5 + 1;
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 10);
            gameObject.Add<Rigidbody>(rigidbody);

            // Create new sphereCollider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f;
            gameObject.Add<Collider>(sphereCollider);

            // New renderer
            Renderer renderer = new Renderer(sphereModel, gameObject.Transform, camera, light, Content, GraphicsDevice, 20f, Content.Load<Texture2D>("Square"), "SimpleShading", 1);
            gameObject.Add<Renderer>(renderer);

            // Adding the game object to the list
            gameObjects.Add(gameObject);
        }
    }
}