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

        //Audio components
        SoundEffect gunSound;
        SoundEffectInstance soundInstance;
        SoundEffect soundExplosion2;
        SoundEffect soundExplosion3;
        SoundEffect engineSound;

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

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.Position = new Vector3(0f, 0f, GameConstants.CameraHeight);

            light = new Light();
            light.Transform = new Transform();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            stars = Content.Load<Texture2D>("b1_stars");
            //lucidaConsole = Content.Load<SpriteFont>("")

            ship = new Ship(Content, camera, GraphicsDevice, light);
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
                asteroidList[i].Transform.Position = new Vector3(0, 0.0f, 0);
                double angle = random.NextDouble() * 2 * Math.PI;
                asteroidList[i].Rigidbody.Velocity = new Vector3(
                    -(float)Math.Sin(angle), 0, (float)Math.Cos(angle)) * (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed);
                asteroidList[i].isActive = true;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            InputManager.Update();
            Time.Update(gameTime);
            ship.Update();

            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Update();
            for (int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Update();

            camera.Transform.Position = new Vector3(ship.Transform.Position.X, ship.Transform.Position.Y, GameConstants.CameraHeight);

            ////ship-asteroid collision check
            //BoundingSphere shipSphere = new BoundingSphere(ship.Transform.Position, ship.Model.Meshes[0].BoundingSphere.Radius * GameConstants.ShipBoundingSphereScale);
            //for (int i = 0; i < asteroidList.Length; i++)
            //{
            //    BoundingSphere b = new BoundingSphere(asteroidList[i].Transform.Position, asteroidList[i].Model.Meshes[0].BoundingSphere.Radius * GameConstants.AsteroidBoundingSphereScale);

            //    if (b.Intersects(shipSphere))
            //    {
            //        //blow up ship
            //        soundExplosion3.Play();
            //        break; //exit the loop
            //    }
            //}

            ////bullet-asteroid collision check
            //for (int i = 0; i < asteroidList.Length; i++)
            //{
            //    if (asteroidList[i].isActive)
            //    {
            //        BoundingSphere asteroidSphere = new BoundingSphere(asteroidList[i].Transform.Position, asteroidList[i].Model.Meshes[0].BoundingSphere.Radius * GameConstants.AsteroidBoundingSphereScale);
            //        for (int j = 0; j < bulletList.Length; j++)
            //        {
            //            if (bulletList[j].isActive)
            //            {
            //                BoundingSphere bulletSphere = new BoundingSphere(bulletList[j].Transform.Position, bulletList[j].Model.Meshes[0].BoundingSphere.Radius);
            //                if (asteroidSphere.Intersects(bulletSphere))
            //                {
            //                    soundExplosion2.Play();
            //                    asteroidList[i].isActive = false;
            //                    bulletList[j].isActive = false;
            //                    break; //no need to check other bullets
            //                }
            //            }
            //        }
            //    }
            //}

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

            //if (Math.Abs((double)ship.Rigidbody.Velocity.Length()) > 0) engineSound.Play();

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
                                // Particles
                                Particle particle = particleManager.getNext();
                                particle.Position = asteroidList[i].Transform.Position;
                                particle.Velocity = new Vector3(random.Next(-5, 5), 2, random.Next(-50, 50));
                                particle.Acceleration = new Vector3(0, 3, 0);
                                particle.MaxAge = random.Next(1, 6);
                                particle.Init();
                                asteroidList[i].isActive = false;
                                bulletList[j].isActive = false;
                                score += GameConstants.KillBonus;
                                break; //no need to check other bullets
                            }
                        }
                    }
                }
            }
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

            // ship, bullets, and asteroids
            ship.Draw();
            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Draw();
            for (int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Draw();
            //particle draw
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            particleEffect.CurrentTechnique = particleEffect.Techniques["particle"];
            particleEffect.CurrentTechnique.Passes[0].Apply();
            particleEffect.Parameters["ViewProj"].SetValue(camera.View * camera.Projection);
            particleEffect.Parameters["World"].SetValue(Matrix.Identity);
            particleEffect.Parameters["CamIRot"].SetValue(Matrix.Invert(Matrix.CreateFromQuaternion(camera.Transform.Rotation)));
            particleEffect.Parameters["Texture"].SetValue(particleTex);
            particleManager.Draw(GraphicsDevice);
            //...
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            base.Draw(gameTime);
        }
    }
}