using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Assignment4
{
    public class Assignment4 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // ****** Scene Items ******************
        public class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }

        Dictionary<string, Scene> scenes;
        Scene currentScene;
        List<GUIElement> guiElements;
        // **************************************

        Random random = new Random();

        Camera camera;
        Light light;

        //Audio components
        SoundEffect gunSound;
        SoundEffectInstance soundInstance;
        SoundEffect soundExplosion2;
        SoundEffect soundExplosion3;

        //Visual components
        Ship ship;
        Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];
        Bullet[] bulletList = new Bullet[GameConstants.NumBullets];

        //Score & background
        int score = 0;
        Texture2D stars;
        SpriteFont lucidaConsole;
        Vector2 scorePosition = new Vector2(100, 50);

        // Particles
        ParticleManager particleManager;
        Texture2D particleTex;
        Effect particleEffect;

        public Assignment4()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();

            scenes = new Dictionary<string, Scene>();
            guiElements = new List<GUIElement>();

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.Position = new Vector3(0.0f, 0.0f, GameConstants.CameraHeight);
            camera.FieldOfView = MathHelper.ToRadians(55.0f);
            camera.FarPlane = 30000.0f;

            light = new Light();
            light.Transform = new Transform();

            SoundEffect activation = Content.Load<SoundEffect>("hyperspace_activate");
            activation.Play();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            stars = Content.Load<Texture2D>("b1_stars");
            lucidaConsole = Content.Load<SpriteFont>("font");

            ship = new Ship(Content, camera, GraphicsDevice, light);
            ship.Transform.LocalScale = new Vector3(1.5f, 1.5f, 1.5f);

            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light);
            ResetAsteroids();

            // Sound effects
            gunSound = Content.Load<SoundEffect>("tx0_fire1");
            soundExplosion2 = Content.Load<SoundEffect>("explosion2");
            soundExplosion3 = Content.Load<SoundEffect>("explosion3");

            // Particle
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particleEffect = Content.Load<Effect>("ParticleShader-complete");
            particleTex = Content.Load<Texture2D>("fire");

            scenes.Add("Gameplay", new Scene(GameplayUpdate, GameplayDraw));
            scenes.Add("GameOver", new Scene(GameoverUpdate, GameoverDraw));
            currentScene = scenes["Gameplay"];
        }

        private void ResetAsteroids()
        {
            float xStart;
            float yStart;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (random.Next(2) == 0) xStart = (float)-GameConstants.PlayfieldSizeX;
                else xStart = (float)GameConstants.PlayfieldSizeX;

                yStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeY;
                asteroidList[i] = new Asteroid(Content, camera, GraphicsDevice, light);
                asteroidList[i].Transform.Position = new Vector3(xStart, yStart, ship.Transform.Position.Z);
                double angle = random.NextDouble() * 2 * Math.PI;
                asteroidList[i].Rigidbody.Velocity = new Vector3(-(float)Math.Sin(angle), (float)Math.Cos(angle), 0) * (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed);
                asteroidList[i].isActive = true;
            }
        }

        protected override void Update(GameTime gameTime)
        {

            InputManager.Update();
            Time.Update(gameTime);
            ship.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            // Shooting bullets
            if (InputManager.IsMouseClicked(0))
            {
                for (int i = 0; i < GameConstants.NumBullets; i++)
                {
                    if (!bulletList[i].isActive)
                    {
                        bulletList[i].Rigidbody.Velocity = (ship.Transform.Forward) * GameConstants.BulletSpeedAdjustment;
                        bulletList[i].Transform.LocalPosition = ship.Transform.Position + (200 * bulletList[i].Transform.Forward);
                        bulletList[i].isActive = true;

                        score -= GameConstants.ShotPenalty;

                        // sound
                        soundInstance = gunSound.CreateInstance();
                        soundInstance.Play();
                        break; //exit the loop  
                    }
                }
            }

            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Update();
            for (int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Update();

            //camera.Transform.Position = new Vector3(ship.Transform.Position.X, ship.Transform.Position.Y, GameConstants.CameraHeight);
            ship.Transform.Position += ship.Rigidbody.Velocity;
            ship.Rigidbody.Velocity *= 0.97f; // ship slows downs gradually

            // Bullet asteroid collision
            Vector3 normal;
            for (int i = 0; i < asteroidList.Length; i++)
            {
                if (asteroidList[i].isActive)
                {
                    for (int j = 0; j < bulletList.Length; j++)
                    {
                        if (bulletList[j].isActive)
                        {
                            if (asteroidList[i].Collider.Collides(bulletList[j].Collider, out normal))
                            {
                                Particle particle = particleManager.getNext();
                                particle.Position = asteroidList[i].Transform.Position;
                                particle.Velocity = new Vector3(random.Next(-5, 5), random.Next(-5, 5), random.Next(-50, 50));
                                particle.Acceleration = new Vector3(0, 3, 0);
                                particle.MaxAge = random.Next(1, 8);
                                particle.Init();
                                asteroidList[i].isActive = false;
                                bulletList[j].isActive = false;

                                score += GameConstants.KillBonus;

                                soundInstance = soundExplosion2.CreateInstance();
                                soundInstance.Play();
                                break;
                            }
                        }
                    }
                }
            }

            // Asteroids collide with ship
            for (int i = 0; i < asteroidList.Length; i++)
            {
                if (asteroidList[i].isActive)
                {
                    if (asteroidList[i].Collider.Collides(ship.Collider, out normal))
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = asteroidList[i].Transform.Position;
                        particle.Velocity = new Vector3(random.Next(-5, 5), random.Next(-5, 5), 0);
                        particle.Acceleration = new Vector3(0, 3, 0);
                        particle.MaxAge = random.Next(1, 8);
                        particle.Init();

                        currentScene = scenes["GameOver"];

                        ship.isActive = false;
                        asteroidList[i].isActive = false;
                        soundInstance = soundExplosion3.CreateInstance();
                        soundInstance.Play();
                        break;
                    }
                }
            }

            // particles update
            particleManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            currentScene.Draw();

            base.Draw(gameTime);
        }

        void GameplayUpdate()
        {
            foreach (GUIElement element in guiElements) element.Update();
        }

        void GameplayDraw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _spriteBatch.Begin();
            _spriteBatch.Draw(stars, new Rectangle(0, 0, 800, 600), Color.White);
            _spriteBatch.DrawString(lucidaConsole, "Score: " + score, scorePosition, Color.LightGreen);
            _spriteBatch.End();

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            particleEffect.CurrentTechnique = particleEffect.Techniques["particle"];
            particleEffect.CurrentTechnique.Passes[0].Apply();
            particleEffect.Parameters["ViewProj"].SetValue(camera.View * camera.Projection);
            particleEffect.Parameters["World"].SetValue(Matrix.Identity);
            particleEffect.Parameters["CamIRot"].SetValue(Matrix.Invert(Matrix.CreateFromQuaternion(camera.Transform.Rotation)));
            particleEffect.Parameters["Texture"].SetValue(particleTex);

            particleManager.Draw(GraphicsDevice);

            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Draw();
            for (int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Draw();

            ship.Draw();
        }

        void GameoverUpdate()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
        }

        void GameoverDraw()
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
            _spriteBatch.DrawString(lucidaConsole, "Game Over, your ship got hit. Press ESC to close the game", scorePosition, Color.DarkRed);
            _spriteBatch.End();
        }
    }
}