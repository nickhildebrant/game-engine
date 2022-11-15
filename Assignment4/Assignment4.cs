using CPI311.GameEngine;
using GoingBeyond4;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Assignment4
{
    public class Assignment4 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Random random = new Random();

        Camera camera;
        Light light;
        Matrix projectionMatrix;
        Matrix viewMatrix;

        //Audio components
        SoundEffect gunSound;
        SoundEffectInstance soundInstance;
        SoundEffect soundExplosion2;
        SoundEffect soundExplosion3;
        SoundEffect engineSound;

        //Visual components
        Ship ship;
        Asteroid asteroid;
        Bullet bullet;
        Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];
        Bullet[] bulletList = new Bullet[GameConstants.NumBullets];

        //Score & background
        int score = 0, playerHealth = 5;
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

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.Position = new Vector3(0.0f, 0.0f, GameConstants.CameraHeight);
            camera.FieldOfView = MathHelper.ToRadians(45.0f);
            camera.AspectRatio = GraphicsDevice.DisplayMode.AspectRatio;
            camera.NearPlane = 10000.0f;
            camera.FarPlane = 30000.0f;

            asteroid = new Asteroid(Content, camera, GraphicsDevice, light);
            //asteroid.Transform.LocalPosition = new Vector3(500, 0, 500);
            asteroid.Transform.LocalScale = new Vector3(3.0f, 3.0f, 3.0f);

            viewMatrix = Matrix.CreateLookAt(camera.Transform.Position, Vector3.Zero, Vector3.Up);

            light = new Light();
            light.Transform = new Transform();

            bullet = new Bullet(Content, camera, GraphicsDevice, light);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            stars = Content.Load<Texture2D>("b1_stars");
            lucidaConsole = Content.Load<SpriteFont>("font");

            ship = new Ship(Content, camera, GraphicsDevice, light);
            ship.Transform.LocalScale = new Vector3(0.2f, 0.2f, 0.2f);

            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light);
            ResetAsteroids(); // look at the below private method

            // Sound effects
            gunSound = Content.Load<SoundEffect>("tx0_fire1");
            soundExplosion2 = Content.Load<SoundEffect>("explosion2");
            soundExplosion3 = Content.Load<SoundEffect>("explosion3");
            engineSound = Content.Load<SoundEffect>("engine_2");

            // *** Particle
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particleEffect = Content.Load<Effect>("ParticleShader-complete");
            particleTex = Content.Load<Texture2D>("fire");
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
                asteroidList[i].Rigidbody.Velocity = new Vector3((-(float)Math.Cos(angle)) * (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed), ((float)Math.Cos(angle)) * (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed), 0);
                asteroidList[i].isActive = true;
            }
        }

        private Matrix[] SetupEffectDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                }
            }
            return absoluteTransforms;
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

            // Add velocity to the current position.
            ship.Transform.Position += ship.Rigidbody.Velocity;
            ship.Rigidbody.Velocity *= 0.95f; // ship slows downs gradually

            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Update();
            for (int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Update();

            //camera.Transform.Position = new Vector3(ship.Transform.Position.X, ship.Transform.Position.Y, GameConstants.CameraHeight);

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
                                particle.Velocity = new Vector3(random.Next(-5, 5), 2, random.Next(-50, 50));
                                particle.Acceleration = new Vector3(0, 3, 0);
                                particle.MaxAge = random.Next(1, 10);
                                particle.Init();
                                asteroidList[i].isActive = false;
                                bulletList[j].isActive = false;
                                score += GameConstants.KillBonus;
                                soundInstance = soundExplosion3.CreateInstance();
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
                        particle.Velocity = new Vector3(random.Next(-5, 5), 2, 0);
                        particle.Acceleration = new Vector3(0, 3, 0);
                        particle.MaxAge = random.Next(5, 10);
                        particle.Init();
                        asteroidList[i].isActive = false;
                        soundInstance = soundExplosion2.CreateInstance();
                        soundInstance.Play();
                        playerHealth -= 1;
                        break;
                    }
                }
            }

            if (playerHealth < 0)
            {
                //Exit();
            }

            //if (Math.Abs((double)ship.Rigidbody.Velocity.Length()) > 0) engineSound.Play();

            // particles update
            particleManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _spriteBatch.Begin();
            _spriteBatch.Draw(stars, new Rectangle(0, 0, 800, 600), Color.White);
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

            base.Draw(gameTime);
        }
    }
}