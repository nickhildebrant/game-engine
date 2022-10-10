using System.Collections.Generic;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment2
{
    public class Assignment2 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;

        Camera camera;
        Transform cameraTransform;

        Light light;

        Model planeModel;
        Transform planeTransform;

        Model playerModel;
        Transform playerTransform;

        Model sunModel, mercModel, earthModel, moonModel;
        Transform sunTransform, mercTransform, earthTransform, moonTransform;

        List<Model> models;

        public Assignment2()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();

            models = new List<Model>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");

            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = new Vector3(-5, 40, 0);
            cameraTransform.Rotate(Vector3.UnitX, -1.6f);
            camera.Transform = cameraTransform;

            // Plane model
            planeModel = Content.Load<Model>("Plane");
            planeTransform = new Transform();
            models.Add(planeModel);

            // Player model
            playerModel = Content.Load<Model>("player");
            playerTransform = new Transform();
            playerTransform.LocalPosition = new Vector3(0, 0, 0);
            models.Add(playerModel);

            // Sun model
            sunModel = Content.Load<Model>("planet");
            sunTransform = new Transform();
            sunTransform.LocalPosition = new Vector3(0, 0, 0);
            models.Add(sunModel);

            // Mercury model
            mercModel = Content.Load<Model>("planet");
            mercTransform = new Transform();
            mercTransform.LocalPosition = new Vector3(0, 0, 0);
            models.Add(mercModel);

            // Earth model
            earthModel = Content.Load<Model>("planet");
            earthTransform = new Transform();
            earthTransform.LocalPosition = new Vector3(0, 0, 0);
            models.Add(earthModel);

            // Moon model
            moonModel = Content.Load<Model>("planet");
            moonTransform = new Transform();
            moonTransform.LocalPosition = new Vector3(0, 0, 0);
            models.Add(moonModel);

            //** For Lighting ************************************
            foreach (Model model in models)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                    }
                }
            }
            // ***************************************************
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            InputManager.Update();
            Time.Update(gameTime);

            // Control zoom (field of view)
            if (InputManager.IsKeyDown(Keys.PageUp) && camera.FieldOfView < 3.0f) camera.FieldOfView += 1.0f * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.PageDown) && camera.FieldOfView > 0.5f) camera.FieldOfView -= 1.0f * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.W)) playerTransform.LocalPosition += Vector3.Forward * 10 * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) playerTransform.LocalPosition += Vector3.Backward * 10 * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.A)) playerTransform.LocalPosition += Vector3.Left * 10 * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.D)) playerTransform.LocalPosition += Vector3.Right * 10 * Time.ElapsedGameTime;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState(); // Fixes model clipping

            planeModel.Draw(planeTransform.World, camera.View, camera.Projection);
            playerModel.Draw(playerTransform.World, camera.View, camera.Projection);
            sunModel.Draw(sunTransform.World, camera.View, camera.Projection);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Zoom: PAGE UP/DOWN", new Vector2(5, 10), Color.Black);
            _spriteBatch.DrawString(font, "Player Position: WASD = " + playerTransform.LocalPosition, new Vector2(5, 30), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}