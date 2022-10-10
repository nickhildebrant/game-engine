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

        bool isFPS = false;

        Camera camera, playerCamera;
        Transform cameraTransform, playerCameraTransform;

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

            // Plane model
            planeModel = Content.Load<Model>("Plane");
            planeTransform = new Transform();
            planeTransform.LocalPosition = new Vector3(0, 0, 0);
            planeTransform.LocalScale = new Vector3(1, 1, 1);
            models.Add(planeModel);

            // Player model
            playerModel = Content.Load<Model>("player");
            playerTransform = new Transform();
            playerTransform.LocalPosition = new Vector3(0, 0, 0);
            models.Add(playerModel);

            // Sun model
            sunModel = Content.Load<Model>("planet");
            sunTransform = new Transform();
            sunTransform.LocalPosition = new Vector3(-5, 20, 0);
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
            moonTransform.LocalPosition = new Vector3(-5, 20, 0);
            models.Add(moonModel);

            // World Camera
            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = new Vector3(-5, 40, 0);
            cameraTransform.Rotate(Vector3.UnitX, -1.55f);
            camera.Transform = cameraTransform;

            // First-person Camera
            playerCamera = new Camera();
            playerCameraTransform = new Transform();
            playerCameraTransform.LocalPosition = new Vector3(0, 5, 0);
            playerCamera.Transform = playerCameraTransform;

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

            // switch camera
            if (InputManager.IsKeyPressed(Keys.Tab)) isFPS = !isFPS;

            Camera cameraInUse;
            if (isFPS) cameraInUse = playerCamera;
            else cameraInUse = camera;

            // Control zoom (field of view)
            if (InputManager.IsKeyDown(Keys.PageDown) && cameraInUse.FieldOfView < 3.0f) cameraInUse.FieldOfView += 1.0f * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.PageUp) && cameraInUse.FieldOfView > 0.5f) cameraInUse.FieldOfView -= 1.0f * Time.ElapsedGameTime;

            // Move the player and camera for fps
            if (InputManager.IsKeyDown(Keys.W))
            {
                playerCameraTransform.LocalPosition += playerTransform.Forward * 10 * Time.ElapsedGameTime;
                playerTransform.LocalPosition += playerTransform.Forward * 10 * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.S))
            {
                playerCameraTransform.LocalPosition += playerTransform.Backward * 10 * Time.ElapsedGameTime;
                playerTransform.LocalPosition += playerTransform.Backward * 10 * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.A))
            {
                playerCameraTransform.LocalPosition += playerTransform.Left * 10 * Time.ElapsedGameTime;
                playerTransform.LocalPosition += playerTransform.Left * 10 * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.D))
            {
                playerCameraTransform.LocalPosition += playerTransform.Right * 10 * Time.ElapsedGameTime;
                playerTransform.LocalPosition += playerTransform.Right * 10 * Time.ElapsedGameTime;
            }

            // rotate the player
            if (InputManager.IsKeyDown(Keys.Left))
            {
                playerTransform.Rotate(Vector3.UnitY, Time.ElapsedGameTime);
                playerCameraTransform.Rotate(Vector3.UnitY, Time.ElapsedGameTime);
            }
            if(InputManager.IsKeyDown(Keys.Right))
            {
                playerTransform.Rotate(Vector3.UnitY, -Time.ElapsedGameTime);
                playerCameraTransform.Rotate(Vector3.UnitY, -Time.ElapsedGameTime);
            }
            if (InputManager.IsKeyDown(Keys.Up)) playerCameraTransform.Rotate(Vector3.UnitX, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Down)) playerCameraTransform.Rotate(Vector3.UnitX, -Time.ElapsedGameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState(); // Fixes model clipping

            Camera cameraInUse;
            if (isFPS) cameraInUse = playerCamera;
            else cameraInUse = camera;
            
            planeModel.Draw(planeTransform.World, cameraInUse.View, cameraInUse.Projection);
            if(!isFPS) playerModel.Draw(playerTransform.World, cameraInUse.View, cameraInUse.Projection);
            sunModel.Draw(sunTransform.World, cameraInUse.View, cameraInUse.Projection);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Zoom: PAGE UP/DOWN", new Vector2(5, 10), Color.Black);
            _spriteBatch.DrawString(font, "Change Camera: TAB", new Vector2(5, 30), Color.Black);
            _spriteBatch.DrawString(font, "Player Position: WASD", new Vector2(5, 50), Color.Black);
            _spriteBatch.DrawString(font, "Player Rotation: Arrow Keys", new Vector2(5, 70), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}