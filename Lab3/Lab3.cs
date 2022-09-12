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
        SpriteFont font;

        // *** Model Data
        Model model;
        float modelScale = 1.0f;
        float yaw = 0, pitch = 0, roll = 0;
        Vector3 modelPosition = new Vector3(0, 0, 0);

        // *** Matrix for 3D
        Matrix world;
        Matrix view;
        Matrix projection;
        bool orderToggle = true;

        // *** Camera Data
        bool cameraMode = true;
        Vector2 cameraSize = new Vector2(1, 1);
        Vector2 cameraCenter = new Vector2(0, 0);
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
            if(InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
            {
                // Change scale of the model with Shift+UP/DOWN
                if (InputManager.IsKeyDown(Keys.Up)) modelScale += 5 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.Down) && modelScale > 0) modelScale -= 5 * Time.ElapsedGameTime;

                // Move center of view
                if (InputManager.IsKeyDown(Keys.W)) cameraCenter -= 2 * Vector2.UnitY * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.A)) cameraCenter += 2 * Vector2.UnitX * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.S)) cameraCenter += 2 * Vector2.UnitY * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.D)) cameraCenter -= 2 * Vector2.UnitX * Time.ElapsedGameTime;
            }
            else if(InputManager.IsKeyDown(Keys.LeftControl) || InputManager.IsKeyDown(Keys.LeftControl))
            {
                // Move width and height of view
                if (InputManager.IsKeyDown(Keys.W) && cameraSize.Y > .1) cameraSize -= 2 * Vector2.UnitY * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.A)) cameraSize += 2 * Vector2.UnitX * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.S)) cameraSize += 2 * Vector2.UnitY * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.D) && cameraSize.X > .1) cameraSize -= 2 * Vector2.UnitX * Time.ElapsedGameTime;
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

                // Changing the model yaw with Insert/Delete
                if (InputManager.IsKeyDown(Keys.Insert)) yaw += 5 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.Delete)) yaw -= 5 * Time.ElapsedGameTime;

                // Changing the model pitch with Home/End
                if (InputManager.IsKeyDown(Keys.Home)) pitch += 5 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.End)) pitch -= 5 * Time.ElapsedGameTime;

                // Changing the model roll with PageUp/PageDown
                if (InputManager.IsKeyDown(Keys.PageUp)) roll += 5 * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.PageDown)) roll -= 5 * Time.ElapsedGameTime;
            }

            // Tab changes the camera mode
            if (InputManager.IsKeyPressed(Keys.Tab)) cameraMode = !cameraMode;

            // Space key changes the order in which the world is created
            if (InputManager.IsKeyPressed(Keys.Space)) orderToggle = !orderToggle;

            if (cameraMode) projection = Matrix.CreatePerspectiveOffCenter(cameraCenter.X - cameraSize.X, cameraCenter.X + cameraSize.X, cameraCenter.Y - cameraSize.Y, cameraCenter.Y + cameraSize.Y, 1f, 100f);
            else projection = Matrix.CreateOrthographicOffCenter(cameraCenter.X - cameraSize.X, cameraCenter.X + cameraSize.X, cameraCenter.Y - cameraSize.Y, cameraCenter.Y + cameraSize.Y, 1f, 100f);

            if (orderToggle) world = Matrix.CreateScale(modelScale) * Matrix.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix.CreateTranslation(modelPosition); // scale * rot * trans
            else world = Matrix.CreateTranslation(modelPosition) * Matrix.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix.CreateScale(modelScale);            // trans * rot * scal

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
            if(cameraMode) _spriteBatch.DrawString(font, "TAB: Current Camera Mode - Perspective", new Vector2(5, 30), Color.Black);
            else _spriteBatch.DrawString(font, "TAB: Current Camera Mode - Orthographic", new Vector2(5, 30), Color.Black);

            // camera movement
            _spriteBatch.DrawString(font, "WASD Keys: Move Camera", new Vector2(5, 70), Color.Black);
            _spriteBatch.DrawString(font, "SHIFT + WASD Keys: Move Camera Center", new Vector2(5, 90), Color.Black);
            _spriteBatch.DrawString(font, "CTRL + WASD Keys: Change Camera Width/Height", new Vector2(5, 110), Color.Black);

            // model movement
            _spriteBatch.DrawString(font, "Arrow Keys: Move Model", new Vector2(5, 150), Color.Black);
            _spriteBatch.DrawString(font, "Insert/Delete: Model Yaw", new Vector2(5, 170), Color.Black);
            _spriteBatch.DrawString(font, "Home/End: Model Pitch", new Vector2(5, 190), Color.Black);
            _spriteBatch.DrawString(font, "PageUp/PageDown: Model Roll", new Vector2(5, 210), Color.Black);
            _spriteBatch.DrawString(font, "SHIFT + Up/Down: Scale Model", new Vector2(5, 230), Color.Black);

            // displaying the camera's center position and size
            _spriteBatch.DrawString(font, "Camera Center: (" + cameraCenter.X.ToString("0.00") + ", " + cameraCenter.Y.ToString("0.00") + ")", new Vector2(550, 10), Color.Black);
            _spriteBatch.DrawString(font, "Camera Width/Height: (" + cameraSize.X.ToString("0.00") + ", " + cameraSize.Y.ToString("0.00") + ")", new Vector2(550, 30), Color.Black);

            // displaying the model's position and rotation
            _spriteBatch.DrawString(font, "Model Position: (" + modelPosition.X.ToString("0.00") + ", " + modelPosition.Y.ToString("0.00") + ")", new Vector2(550, 70), Color.Black);
            _spriteBatch.DrawString(font, "Model Rotation: (" + yaw.ToString("0.00") + ", " + pitch.ToString("0.00") + ", " + roll.ToString("0.00") + ")", new Vector2(550, 90), Color.Black);
            _spriteBatch.DrawString(font, "Model Scale: (" + modelScale.ToString("0.00") + ", " + modelScale.ToString("0.00") + ", " + modelScale.ToString("0.00") + ")", new Vector2(550, 110), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}