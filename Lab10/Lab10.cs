using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPI311.Labs
{
    public class Lab10 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        TerrainRenderer terrain;
        Camera camera;
        Effect effect;

        public Lab10()
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

            terrain = new TerrainRenderer(Content.Load<Texture2D>("Heightmap"), Vector2.One * 100, Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("Normalmap");

            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1, 10, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0f,.2f,0f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(1f, 1f, 0f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(.1f, .1f, .1f));
            effect.Parameters["Shininess"].SetValue(20f);
            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5 + Vector3.Right * 5 + Vector3.Up * 5;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            InputManager.Update();

            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += 5 * camera.Transform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += 5 * camera.Transform.Backward * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.Rotate(Vector3.Down, Time.ElapsedGameTime);

            camera.Transform.LocalPosition = new Vector3(camera.Transform.LocalPosition.X, terrain.GetAltitude(camera.Transform.LocalPosition), camera.Transform.LocalPosition.Z) + Vector3.Up;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["LightPosition"].SetValue(camera.Transform.Position + Vector3.Up * 10);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();
            }

            base.Draw(gameTime);
        }
    }
}