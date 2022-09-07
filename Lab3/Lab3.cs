using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace CPI311.Labs
{
    public class Lab3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // *** Lab3
        Model model;
        Vector3 modelPosition = new Vector3(0, 0, 0);

        // *** Matrix for 3D
        Matrix world;
        Matrix view;
        Matrix projection;

        // *** Camera Data
        Vector3 cameraPosition = new Vector3(0, 0, 5);

        public Lab3()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Torus");

            // ************* Add lighting *******************
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
            }
            // **********************************************
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            // Move camera with WASD in 3D world
            if (InputManager.IsKeyDown(Keys.W)) cameraPosition += 5 * Vector3.Up * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.A)) cameraPosition += 5 * Vector3.Left * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) cameraPosition += 5 * Vector3.Down * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.D)) cameraPosition += 5 * Vector3.Right * Time.ElapsedGameTime;

            // Move model in 3D world
            if (InputManager.IsKeyDown(Keys.Up)) modelPosition += 5 * Vector3.Up * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Down)) modelPosition += 5 * Vector3.Down * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Left)) modelPosition += 5 * Vector3.Left * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Right)) modelPosition += 5 * Vector3.Right * Time.ElapsedGameTime;


            world = Matrix.CreateScale(1.0f) * Matrix.CreateFromYawPitchRoll(0, 0, 0) * Matrix.CreateTranslation(modelPosition);
            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, -1), new Vector3(0, 1, 0));
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.33f, 0.01f, 1000f);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            model.Draw(world, view, projection);

            base.Draw(gameTime);
        }
    }
}