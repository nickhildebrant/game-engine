using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Final
{
    public class Final : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SpriteFont font;

        Texture2D ceiling;
        Texture2D paper;

        SoundEffectInstance soundInstance;
        SoundEffect paperPickupSound;
        SoundEffect officeSounds;

        TerrainRenderer terrain;
        Effect effect;

        // Particles
        ParticleManager particleManager;
        Texture2D particleTex;
        Effect particleEffect;

        Random random = new Random();

        Camera camera;
        Light light;

        Player player;
        Boss boss;
        ExitSign exitSign;

        List<Prize> assigments;

        int timeLeft = 0;
        bool canPickup = false;
        bool isExiting = false;
        int collectedPapers = 0;
        bool isOutofTime = false;

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

        // Menu Items
        Button playButton;
        Button fullscreenButton;
        Texture2D logo;

        public Final()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);

            scenes = new Dictionary<string, Scene>();
            assigments = new List<Prize>();
            guiElements = new List<GUIElement>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ceiling = Content.Load<Texture2D>("ceiling");
            paper = Content.Load<Texture2D>("Square");

            font = Content.Load<SpriteFont>("font");

            paperPickupSound = Content.Load<SoundEffect>("Paper Sound Effect");
            officeSounds = Content.Load<SoundEffect>("Work Office Sounds Ambience (Background Sound Effect)");
            soundInstance = officeSounds.CreateInstance();
            soundInstance.IsLooped = true;
            soundInstance.Volume = .50f;
            soundInstance.Play();

            terrain = new TerrainRenderer(Content.Load<Texture2D>("mazeH"), Vector2.One * 100, Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("mazeN");
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1, 12, 1);//12, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));//new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.2f, 0.1f));
            effect.Parameters["Shininess"].SetValue(10f);

            // Particle
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particleEffect = Content.Load<Effect>("ParticleShader-complete");
            particleTex = Content.Load<Texture2D>("taxes");

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Up * 10;
            camera.NearPlane = 0.0001f;

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * 200 + Vector3.Right * 5 + Vector3.Up * 5;
            light.Transform.Rotate(Vector3.Left, MathHelper.PiOver2);// - 0.2f);

            player = new Player(terrain, Content, camera, GraphicsDevice, light);
            camera.Transform.Parent = player.Transform;

            boss = new Boss(terrain, Content, camera, GraphicsDevice, light, player);

            for(int i = 0; i < 3; i++)
            {
                assigments.Add(new Prize(terrain, Content, camera, GraphicsDevice, light));
            }

            exitSign = new ExitSign(terrain, Content, camera, GraphicsDevice, light);

            fullscreenButton = new Button();
            fullscreenButton.Text = "  Fullscreen Mode";
            fullscreenButton.Texture = paper;
            fullscreenButton.Bounds = new Rectangle(ScreenManager.Width / 2 - 96, 320, 192, 24);
            fullscreenButton.Action += FullScreen;
            guiElements.Add(fullscreenButton);

            playButton = new Button();
            playButton.Text = "           Play";
            playButton.Texture = paper;
            playButton.Bounds = new Rectangle(ScreenManager.Width / 2 - 96, 352, 192, 24);
            playButton.Action += PlayGame;
            guiElements.Add(playButton);

            logo = Content.Load<Texture2D>("officeLogo");

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Gameplay", new Scene(PlayUpdate, PlayDraw));
            scenes.Add("GameOver", new Scene(GameOverUpdate, GameOverDraw));
            scenes.Add("GameWin", new Scene(WinUpdate, WinDraw));
            currentScene = scenes["Menu"];
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            InputManager.Update();

            if (soundInstance.State != SoundState.Playing) soundInstance.Play();

            currentScene.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            currentScene.Draw();

            base.Draw(gameTime);
        }

        void FullScreen(GUIElement element)
        {
            ScreenManager.Setup(1920, 1080);
            ScreenManager.IsFullScreen = true;
            ScreenManager.Width = 1920;
            ScreenManager.Height = 1080;
            currentScene = scenes["Gameplay"];
        }

        void PlayGame(GUIElement element)
        {
            currentScene = scenes["Gameplay"];
        }

        void MainMenuUpdate()
        {
            foreach (GUIElement element in guiElements) element.Update();
        }

        void MainMenuDraw()
        {
            GraphicsDevice.Clear(Color.ForestGreen);
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            _spriteBatch.Begin();
            foreach (GUIElement element in guiElements) element.Draw(_spriteBatch, font);
            _spriteBatch.Draw(logo, new Rectangle(ScreenManager.Width/2 - 128, 32, 256, 256), Color.Yellow);
            _spriteBatch.DrawString(font, "OVERTIME", new Vector2(ScreenManager.Width / 2 - 56, 292), Color.Gold);
            _spriteBatch.DrawString(font, "Collect your work as quickly as possible to leave work early,", new Vector2(ScreenManager.Width / 2 - 292, 392), Color.Gold);
            _spriteBatch.DrawString(font, "but don't get caught by your boss!", new Vector2(ScreenManager.Width / 2 - 162, 416), Color.Gold);
            _spriteBatch.DrawString(font, "WASD to move, CTRL to crouch/sneak, ARROW KEYS to look around", new Vector2(ScreenManager.Width / 2 - 352, 440), Color.Gold);
            _spriteBatch.End();
        }

        void PlayUpdate()
        {
            // Movement
            if (InputManager.IsKeyDown(Keys.LeftControl))
            {
                camera.Transform.LocalPosition = new Vector3(camera.Transform.LocalPosition.X, 2, camera.Transform.LocalPosition.Z);

                if (InputManager.IsKeyDown(Keys.W) && terrain.GetAltitude(camera.Transform.LocalPosition + camera.Transform.Forward * 2) < 0.25f)
                    camera.Transform.LocalPosition += camera.Transform.Forward * Time.ElapsedGameTime * 4f; // move forward

                if (InputManager.IsKeyDown(Keys.S) && terrain.GetAltitude(camera.Transform.LocalPosition + camera.Transform.Backward * 2) < 0.25f)
                    camera.Transform.LocalPosition += camera.Transform.Backward * Time.ElapsedGameTime * 4f; // move backward

                if (InputManager.IsKeyDown(Keys.A) && terrain.GetAltitude(camera.Transform.LocalPosition + camera.Transform.Left * 2) < 0.25f)
                    camera.Transform.LocalPosition += camera.Transform.Left * Time.ElapsedGameTime * 4f; // move right

                if (InputManager.IsKeyDown(Keys.D) && terrain.GetAltitude(camera.Transform.LocalPosition + camera.Transform.Right * 2) < 0.25f)
                    camera.Transform.LocalPosition += camera.Transform.Right * Time.ElapsedGameTime * 4f; // move left
            }
            else
            {
                camera.Transform.LocalPosition = new Vector3(camera.Transform.LocalPosition.X, 5, camera.Transform.LocalPosition.Z);

                if (InputManager.IsKeyDown(Keys.W) && terrain.GetAltitude(camera.Transform.LocalPosition + camera.Transform.Forward * 2) < 0.25f)
                    camera.Transform.LocalPosition += camera.Transform.Forward * Time.ElapsedGameTime * 10f; // move forward

                if (InputManager.IsKeyDown(Keys.S) && terrain.GetAltitude(camera.Transform.LocalPosition + camera.Transform.Backward * 2) < 0.25f)
                    camera.Transform.LocalPosition += camera.Transform.Backward * Time.ElapsedGameTime * 10f; // move backward

                if (InputManager.IsKeyDown(Keys.A) && terrain.GetAltitude(camera.Transform.LocalPosition + camera.Transform.Left * 2) < 0.25f)
                    camera.Transform.LocalPosition += camera.Transform.Left * Time.ElapsedGameTime * 10f; // move right

                if (InputManager.IsKeyDown(Keys.D) && terrain.GetAltitude(camera.Transform.LocalPosition + camera.Transform.Right * 2) < 0.25f)
                    camera.Transform.LocalPosition += camera.Transform.Right * Time.ElapsedGameTime * 10f; // move left
            }

            player.Transform.LocalPosition = camera.Transform.LocalPosition;

            // Camera movement
            if (InputManager.IsKeyDown(Keys.Left))
            {
                player.Transform.Rotate(Vector3.UnitY, 3f * Time.ElapsedGameTime);
                light.Transform.Rotate(Vector3.UnitY, 3f * Time.ElapsedGameTime);
            }

            if (InputManager.IsKeyDown(Keys.Right))
            {
                player.Transform.Rotate(Vector3.UnitY, -3f * Time.ElapsedGameTime);
                light.Transform.Rotate(Vector3.UnitY, -3f * Time.ElapsedGameTime);
            }

            if (InputManager.IsKeyDown(Keys.Up))
            {
                camera.Transform.Rotate(Vector3.UnitX, 2f * Time.ElapsedGameTime);
                light.Transform.Rotate(Vector3.UnitX, 2f * Time.ElapsedGameTime);
            }

            if (InputManager.IsKeyDown(Keys.Down))
            {
                camera.Transform.Rotate(Vector3.UnitX, -2f * Time.ElapsedGameTime);
                light.Transform.Rotate(Vector3.UnitX, -2f * Time.ElapsedGameTime);
            }

            boss.Update();

            canPickup = false;

            for (int i = 0; i < assigments.Count; i++)
            {
                assigments[i].Update();

                if (Vector3.Distance(assigments[i].Transform.LocalPosition, camera.Transform.LocalPosition) <= 6f)
                {
                    //Debug.WriteLine("Pickup Item");

                    canPickup = true;

                    if (InputManager.IsKeyPressed(Keys.F))
                    {
                        paperPickupSound.Play();

                        Particle particle = particleManager.getNext();
                        particle.Position = assigments[i].Transform.LocalPosition;
                        particle.Velocity = new Vector3(random.Next(-5, 5), random.Next(-5, 5), random.Next(-50, 50));
                        particle.Acceleration = new Vector3(0, 3, 0);
                        particle.MaxAge = random.Next(1, 8);
                        particle.Init();

                        assigments.Remove(assigments[i]);
                        collectedPapers++;
                    }
                }
            }

            if (isExiting)
            {
                exitSign.Update();

                if (Vector3.Distance(exitSign.Transform.LocalPosition, camera.Transform.LocalPosition) <= 6f)
                {
                    //Debug.WriteLine("Pickup Item");

                    canPickup = true;

                    if (InputManager.IsKeyPressed(Keys.F))
                    {
                        Debug.WriteLine("You win!!");
                        currentScene = scenes["GameWin"];
                        timeLeft = (90 - Time.TotalGameTime.Seconds);
                        Content.Load<SoundEffect>("victory").Play();
                    }
                }
            }

            // 3 Papers Collected
            if (collectedPapers == 3)
            {
                // Spawn in exit door
                isExiting = true;
                boss.speed = 15f;
            }

            // Times Up - GameOver
            if (90 - Time.TotalGameTime.Seconds <= 0)
            {
                isOutofTime = true;
                currentScene = scenes["GameOver"];
                Content.Load<SoundEffect>("lost").Play();
            }

            // Collision with boss - GameOver
            if (Vector3.Distance(player.Transform.LocalPosition, boss.Transform.LocalPosition) < 5.0f)
            {
                Debug.WriteLine("Gameover, the boss caught you");
                currentScene = scenes["GameOver"];
                Content.Load<SoundEffect>("lost").Play();
            }
        }

        void PlayDraw()
        {
            GraphicsDevice.Clear(Color.DimGray);

            _spriteBatch.Begin();
            _spriteBatch.Draw(ceiling, new Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height/2), Color.LightYellow);
            _spriteBatch.End();

            GraphicsDevice.DepthStencilState = new DepthStencilState();

            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);

            particleManager.Draw(GraphicsDevice);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();

                boss.Draw();

                if (isExiting) exitSign.Draw();

                foreach (Prize paper in assigments) { paper.Draw(); }
            }

            particleEffect.CurrentTechnique = particleEffect.Techniques["particle"];
            particleEffect.CurrentTechnique.Passes[0].Apply();
            particleEffect.Parameters["ViewProj"].SetValue(camera.View * camera.Projection);
            particleEffect.Parameters["World"].SetValue(Matrix.Identity);
            particleEffect.Parameters["CamIRot"].SetValue(Matrix.Invert(Matrix.CreateFromQuaternion(camera.Transform.Rotation)));
            particleEffect.Parameters["Texture"].SetValue(particleTex);

            _spriteBatch.Begin();
            if (!isExiting) for (int i = 0; i < collectedPapers; i++) _spriteBatch.Draw(paper, new Rectangle(192 + i * 28, 8, 24, 24), Color.White);
            else _spriteBatch.DrawString(font, "SNEAK OUT TO THE EXIT!", new Vector2(192, 10), Color.Yellow);

            _spriteBatch.DrawString(font, "Papers Collected: ", new Vector2(5, 10), Color.Goldenrod);
            _spriteBatch.DrawString(font, "Time Remaining: " + (90 - Time.TotalGameTime.Seconds), new Vector2(5, 35), Color.Gold);
            if (canPickup) _spriteBatch.DrawString(font, "Press F to pickup", new Vector2(ScreenManager.Width / 2, ScreenManager.Height / 2), Color.Yellow);
            _spriteBatch.End();
        }

        void GameOverUpdate()
        {
            playButton.Update();
        }

        void GameOverDraw()
        {
            GraphicsDevice.Clear(Color.DarkRed);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "YOU LOSE!", new Vector2(108, 100), Color.White);
            if(isOutofTime)_spriteBatch.DrawString(font, "You ran out of time, and now you must work Overtime.", new Vector2(108, 150), Color.White);
            else _spriteBatch.DrawString(font, "The boss caught you, and now you must work Overtime.", new Vector2(108, 200), Color.White);
            _spriteBatch.DrawString(font, "Play Again?", new Vector2(108, 250), Color.White);
            _spriteBatch.DrawString(font, "Press ESC to exit", new Vector2(108, 300), Color.White);

            //_spriteBatch.Draw(Content.Load<Texture2D>("businessGuy"), new Rectangle(ScreenManager.Width/2, 100, 1024, 1024), Color.White);
            _spriteBatch.End();
        }

        void WinUpdate()
        {
            playButton.Update();
        }

        void WinDraw()
        {
            GraphicsDevice.Clear(Color.DarkOliveGreen);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(font, "YOU WIN!", new Vector2(108, 100), Color.White);
            _spriteBatch.DrawString(font, "You got out of work with " + timeLeft + " seconds left!", new Vector2(108, 150), Color.White);
            _spriteBatch.DrawString(font, "Play Again?", new Vector2(108, 250), Color.White);
            _spriteBatch.DrawString(font, "Press ESC to exit", new Vector2(108, 300), Color.White);
            
            _spriteBatch.End();
        }
    }
}