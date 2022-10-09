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
            cameraTransform.LocalPosition = new Vector3(0, 0, 5);
            camera.Transform = cameraTransform;

            // Plane model
            planeModel = Content.Load<Model>("Plane");
            planeTransform = new Transform();

            // Player model
            playerModel = Content.Load<Model>("Torus");
            playerTransform = new Transform();
            models.Add(playerModel);

            //** For Lighting ************************************
            foreach(Model model in models)
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState(); // Fixes model clipping

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Zoom: PAGE UP/DOWN", new Vector2(5, 10), Color.Black);
            _spriteBatch.End();

            //plane.Draw(planeTransform.World, camera.View, camera.Projection);
            playerModel.Draw(playerTransform.World, camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}