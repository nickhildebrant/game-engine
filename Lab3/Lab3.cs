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
        bool cameraMode = true;

        bool orderToggle = true;

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

            // If the shift key is held down
            if(InputManager.IsKeyDown(Keys.LeftShift))
            {
                // Change scale of the model with Shift+UP/DOWN
                if (InputManager.IsKeyDown(Keys.Up)) modelScale += 5 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.Down) && modelScale > 0) modelScale -= 5 * Time.ElapsedGameTime;
            }
            else // The shift key is not being held down
            {
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
            }

            // Tab changes the camera mode
            if (InputManager.IsKeyPressed(Keys.Tab)) cameraMode = !cameraMode;

            // Space key changes the order in which the world is created
            if (InputManager.IsKeyPressed(Keys.Space)) orderToggle = !orderToggle;

            if (cameraMode) projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.33f, 0.01f, 1000f);
            else projection = Matrix.CreateOrthographic(MathHelper.PiOver2, 1.33f, 0.01f, 1000f);

            if(orderToggle) world = Matrix.CreateScale(modelScale) * Matrix.CreateFromYawPitchRoll(0, 0, 0) * Matrix.CreateTranslation(modelPosition); // scale * rot * trans
            else world = Matrix.CreateTranslation(modelPosition) * Matrix.CreateFromYawPitchRoll(0, 0, 0) * Matrix.CreateScale(modelScale);            // trans * rot * scal

            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, -1), new Vector3(0, 1, 0));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState(); // Fixes model clipping

            model.Draw(world, view, projection);

            _spriteBatch.Begin();

            // printing the method of multiplying matricies
            if(orderToggle) _spriteBatch.DrawString(font, "SPACE: World = Scale * Rotation * Translation", new Vector2(5, 10), Color.Black);
            else _spriteBatch.DrawString(font, "SPACE: World = Translation * Rotation * Scale", new Vector2(5, 10), Color.Black);

            // printing camera mode
            if(cameraMode) _spriteBatch.DrawString(font, "TAB: Current Camera Mode: Perspective", new Vector2(5, 30), Color.Black);
            else _spriteBatch.DrawString(font, "TAB: Current Camera Mode: Orthographic", new Vector2(5, 30), Color.Black);

            // Movement
            _spriteBatch.DrawString(font, "WASD Keys: Move Camera", new Vector2(5, 50), Color.Black);

            _spriteBatch.DrawString(font, "Arrow Keys: Move Model", new Vector2(5, 130), Color.Black);
            _spriteBatch.DrawString(font, "Model Position: (" + modelPosition.X + ", " + modelPosition.Y + ")", new Vector2(5, 150), Color.Black);
            _spriteBatch.DrawString(font, "Shift + Up/Down: Scale Model", new Vector2(5, 170), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}