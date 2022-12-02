using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System.Collections.Generic;

namespace Assignment5
{
    public class Assignment5 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D stars;

        // *** Assignment5
        TerrainRenderer terrain;
        Effect effect;

        Camera camera;
        Light light;

        Player player;
        Bomb bomb;
        List<Agent> agents;

        SpriteFont font;

        int agentCollisions = 0;

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

        public Assignment5()
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            stars = Content.Load<Texture2D>("b1_stars");

            font = Content.Load<SpriteFont>("font");

            agents = new List<Agent>();

            terrain = new TerrainRenderer(Content.Load<Texture2D>("mazeH2"), Vector2.One * 100, Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("mazeN2");
            float height = terrain.GetHeight(new Vector2(0.5f, 0.5f));
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1, 5, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Up * 60;
            camera.Transform.Rotate(Vector3.Left, MathHelper.PiOver2);// - 0.2f);
            camera.Position = new Vector2(0.75f, 0f);
            camera.Size = new Vector2(0.2f, 0.2f);

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * 200 + Vector3.Right * 5 + Vector3.Up * 5;

            player = new Player(terrain, Content, camera, GraphicsDevice, light);

            bomb = new Bomb(terrain, Content, camera, GraphicsDevice, light, player);

            for (int i = 0; i < 3; i++) agents.Add(new Agent(terrain, Content, camera, GraphicsDevice, light));

            scenes.Add("Gameplay", new Scene(GameplayUpdate, GameplayDraw));
            scenes.Add("GameOver", new Scene(GameoverUpdate, GameoverDraw));
            currentScene = scenes["Gameplay"];

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            InputManager.Update();

            if (InputManager.IsKeyDown(Keys.Up)) camera.Transform.Rotate(Vector3.Right, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Down)) camera.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime);

            if (InputManager.IsKeyDown(Keys.U)) light.Transform.LocalPosition += light.Transform.Forward * 5f * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.I)) light.Transform.LocalPosition += light.Transform.Backward * 5f * Time.ElapsedGameTime;

            player.Update();

            bomb.Update();

            foreach (Agent agent in agents)
            {
                agent.Update();
                if(agent.CheckCollision(player)) agentCollisions++;
            }

            if(Vector3.Distance(player.Transform.Position, bomb.Transform.Position) <= 2.25f) currentScene = scenes["GameOver"];

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
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            _spriteBatch.Begin();
            _spriteBatch.Draw(stars, new Rectangle(0, 0, 800, 600), Color.White);
            _spriteBatch.End();

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

                player.Draw();
                bomb.Draw();

                foreach (Agent agent in agents) agent.Draw();
            }

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Aliens Caught: " + agentCollisions, new Vector2(5, 10), Color.LightGreen);
            _spriteBatch.DrawString(font, "Time Played: " + (int)Time.TotalGameTime.TotalSeconds, new Vector2(5, 35), Color.LightGreen);
            _spriteBatch.End();
        }

        void GameoverUpdate()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
        }

        void GameoverDraw()
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
            _spriteBatch.Draw(stars, new Rectangle(0, 0, 800, 600), Color.White);
            _spriteBatch.DrawString(font, "Game Over, you got hit by a bomb. Press ESC to close the game", new Vector2(120, ScreenManager.Height/2), Color.Green);
            _spriteBatch.End();
        }
    }
}