using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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

        Camera camera;
        Light light;

        Player player;
        Boss boss;

        List<Prize> assigments;

        bool canPickup = false;
        int collectedPapers = 0;

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
            terrain.Transform.LocalScale *= new Vector3(1, 2, 1);//12, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));//new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.2f, 0.1f));
            effect.Parameters["Shininess"].SetValue(10f);

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
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            InputManager.Update();

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
                player.Transform.Rotate(Vector3.UnitY, 2f * Time.ElapsedGameTime); 
                light.Transform.Rotate(Vector3.UnitY, 2f * Time.ElapsedGameTime); 
            }

            if (InputManager.IsKeyDown(Keys.Right)) 
            { 
                player.Transform.Rotate(Vector3.UnitY, -2f * Time.ElapsedGameTime);
                light.Transform.Rotate(Vector3.UnitY, -2f * Time.ElapsedGameTime);
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
                        assigments.Remove(assigments[i]);
                        collectedPapers++;
                    }
                }
            }

            // 3 Papers Collected
            if(collectedPapers == 3)
            {
                // Spawn in exit door
            }

            // Times Up - GameOver
            if(90 - Time.TotalGameTime.Seconds <= 0)
            {
                currentScene = null;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            //_spriteBatch.Begin();
            //_spriteBatch.Draw(ceiling, new Rectangle(0, 0, 400, 300), Color.White);
            //_spriteBatch.End();

            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();

                boss.Draw();

                foreach (Prize paper in assigments) { paper.Draw(); }
            }

            _spriteBatch.Begin();
            for(int i = 0; i < collectedPapers; i++) _spriteBatch.Draw(paper, new Rectangle(132 + i * 28, 4, 24, 24), Color.White);
            _spriteBatch.DrawString(font, "Papers Collected: ", new Vector2(5, 10), Color.Goldenrod);
            _spriteBatch.DrawString(font, "Time Remaining: " + (90 - Time.TotalGameTime.Seconds), new Vector2(5, 35), Color.Gold);
            if(canPickup) _spriteBatch.DrawString(font, "Press F to pickup", new Vector2(ScreenManager.Width / 2, ScreenManager.Height / 2), Color.Yellow);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}