using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assignment3
{
    public class Assignment3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;

        Random random = new Random();
        
        bool isShowing = true;
        int numSpheres = 1;
        float speed = 1.0f;

        float averageFrames = 60;
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

            Time.Update(gameTime);
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
            if (InputManager.IsKeyDown(Keys.Up)) { numSpheres++; AddGameObject(); }
            if (InputManager.IsKeyDown(Keys.Down) && numSpheres > 0) { gameObjects.RemoveAt(numSpheres-1); numSpheres--; }

            // Update each GameObject
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update();
                gameObject.Rigidbody.Velocity *= speed;
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

                averageCollisions = totalCollisions / 5;

                System.Threading.Thread.Sleep(5000);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            if (frameRates.Count == 10)
            {
                frameRates.Dequeue();
            }

            if (Time.TotalGameTime.Seconds % 1 == 0) frameRates.Enqueue(1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (Time.TotalGameTime.Seconds % 10 == 0)
            {
                float frameTotal = 0;
                foreach (float count in frameRates)
                {
                    frameTotal += count;
                }

                averageFrames = frameTotal / frameRates.Count;
            }

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw();
            }

            _spriteBatch.DrawString(font, "SHIFT - Show/Hide Details", new Vector2(5, 10), Color.Black);

            if(isShowing)
            {
                _spriteBatch.DrawString(font, "Arrows (LEFT/RIGHT) - Speed: " + speed.ToString("0.0"), new Vector2(5, 25), Color.Black);
                _spriteBatch.DrawString(font, "Arrows (UP/DOWN) - # of Spheres: " + numSpheres, new Vector2(5, 40), Color.Black);
                _spriteBatch.DrawString(font, "Average Frame Rate: " + averageFrames.ToString("0.0"), new Vector2(5, 55), Color.Black);
                _spriteBatch.DrawString(font, "Average Collisions: " + averageCollisions, new Vector2(5, 70), Color.Black);
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