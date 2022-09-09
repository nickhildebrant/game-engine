using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using SharpDX.Direct3D9;

namespace CPI311.Labs
{
    public class Lab3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // *** Lab3
        Model model;
        float modelScale = 1.0f;
        Vector3 modelPosition = new Vector3(0, 0, 0);

        SpriteFont font;
        string cameraString = "Perspective";

        // *** Matrix for 3D
        Matrix world;
        Matrix view;
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.33f, 0.01f, 1000f);

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
            font = Content.Load<SpriteFont>("font");

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

            // Move model in 3D world with the arrow keys
            if (InputManager.IsKeyDown(Keys.Up)) modelPosition += 5 * Vector3.Up * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Down)) modelPosition += 5 * Vector3.Down * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Left)) modelPosition += 5 * Vector3.Left * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Right)) modelPosition += 5 * Vector3.Right * Time.ElapsedGameTime;

            // Change scale of the model with Shift+UP/DOWN
            if (InputManager.IsKeyDown(Keys.Up) && InputManager.IsKeyDown(Keys.LeftShift)) modelScale += 5 * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Down) && InputManager.IsKeyDown(Keys.LeftShift)) modelScale -= 5 * Time.ElapsedGameTime;

            // Tab changes the camera mode
            if (InputManager.IsKeyPressed(Keys.Tab))
            {
                if (cameraString == "Perspective")  { projection = Matrix.CreateOrthographic(MathHelper.PiOver2, 1.33f, 0.01f, 1000f); cameraString = "Orthographic"; }
                if (cameraString == "Orthographic") { projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.33f, 0.01f, 1000f); cameraString = "Perspective"; }
            }

            world = Matrix.CreateScale(modelScale) * Matrix.CreateFromYawPitchRoll(0, 0, 0) * Matrix.CreateTranslation(modelPosition);
            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, -1), new Vector3(0, 1, 0));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState(); // Fixes model clipping

            model.Draw(world, view, projection);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Current Camera Mode: " + cameraString, new Vector2(5, 5), Color.Black);
            _spriteBatch.DrawString(font, "Tab: Change Camera Mode", new Vector2(5, 20), Color.Black);
            _spriteBatch.DrawString(font, "WASD Keys: Move Camera", new Vector2(5, 35), Color.Black);

            _spriteBatch.DrawString(font, "Model Position: (" + modelPosition.X + ", " + modelPosition.Y + ")", new Vector2(5, 50), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}