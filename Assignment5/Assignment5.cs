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

        // *** Assignment5
        TerrainRenderer terrain;
        Effect effect;

        Camera camera, mapCamera;
        Light light;

        Player player;
        Bomb bomb;

        List<Agent> agents;

        List<Transform> transforms;
        List<Camera> cameras;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            cameras = new List<Camera>();
            transforms = new List<Transform>();

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
            camera.Transform.LocalPosition = Vector3.Up * 50;
            camera.Transform.Rotate(Vector3.Left, MathHelper.PiOver2);
            camera.Position = new Vector2(-0.1f, 0f);
            camera.AspectRatio = camera.Viewport.AspectRatio;
            cameras.Add(camera);

            mapCamera = new Camera();
            mapCamera.Transform = new Transform();
            mapCamera.Transform.LocalPosition = Vector3.Up * 60;
            mapCamera.Transform.Rotate(Vector3.Left, MathHelper.PiOver2);// - 0.2f);
            mapCamera.Position = new Vector2(0.75f, 0f);
            mapCamera.Size = new Vector2(0.2f, 0.2f);
            cameras.Add(mapCamera);

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * 200 + Vector3.Right * 5 + Vector3.Up * 5;

            player = new Player(terrain, Content, mapCamera, GraphicsDevice, light);

            bomb = new Bomb(terrain, Content, mapCamera, GraphicsDevice, light, player);

            for (int i = 0; i < 3; i++) agents.Add(new Agent(terrain, Content, mapCamera, GraphicsDevice, light));

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            InputManager.Update();

            if (InputManager.IsKeyDown(Keys.Up)) mapCamera.Transform.Rotate(Vector3.Right, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Down)) mapCamera.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime);

            if (InputManager.IsKeyDown(Keys.U)) light.Transform.LocalPosition += light.Transform.Forward * 5f * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.I)) light.Transform.LocalPosition += light.Transform.Backward * 5f * Time.ElapsedGameTime;

            player.Update();

            bomb.Update();

            foreach (Agent agent in agents)
            {
                agent.Update();
                agent.CheckCollision(player);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            //GraphicsDevice.Viewport = cameras[0].Viewport; // draw items using cameras[0] 
            //GraphicsDevice.Viewport = cameras[1].Viewport; // draw items using cameras[1]

            //foreach (Camera cam in cameras)
            //{
            //    effect.Parameters["View"].SetValue(cam.View);
            //    effect.Parameters["Projection"].SetValue(cam.Projection);
            //    effect.Parameters["World"].SetValue(terrain.Transform.World);
            //    effect.Parameters["CameraPosition"].SetValue(cam.Transform.Position);
            //    effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
            //    effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);

            //    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        terrain.Draw();
            //    }

            //    player.Get<Renderer>().Camera = cam;
            //    agent.Get<Renderer>().Camera = cam;

            //    player.Draw();
            //    agent.Draw();
            //}

            effect.Parameters["View"].SetValue(mapCamera.View);
            effect.Parameters["Projection"].SetValue(mapCamera.Projection);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["CameraPosition"].SetValue(mapCamera.Transform.Position);
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

            base.Draw(gameTime);
        }
    }
}