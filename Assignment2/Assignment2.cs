using System;
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

        float simSpeed = 1.0f;
        
        bool isFPS = false;

        Camera camera, playerCamera;
        Transform cameraTransform, playerCameraTransform;

        Model planeModel;
        Transform planeTransform;

        Model playerModel;
        Transform playerTransform;

        float orbitAngle = 0f;
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
            planeTransform.LocalScale = new Vector3(3, 1, 3);
            models.Add(planeModel);

            // Player model
            playerModel = Content.Load<Model>("player");
            playerTransform = new Transform();
            playerTransform.LocalPosition = new Vector3(0, 0, 0);
            models.Add(playerModel);
            (playerModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = new Vector3(0.5f, 0f, 0.8f);

            // Sun model
            sunModel = Content.Load<Model>("planet");
            sunTransform = new Transform();
            sunTransform.LocalPosition = new Vector3(0, 30, 0);
            sunTransform.LocalScale = new Vector3(5, 5, 5);
            models.Add(sunModel);

            // Mercury model
            mercModel = Content.Load<Model>("planet");
            mercTransform = new Transform();
            mercTransform.LocalScale = new Vector3(2, 2, 2);
            models.Add(mercModel);

            // Earth model
            earthModel = Content.Load<Model>("planet");
            earthTransform = new Transform();
            earthTransform.LocalScale = new Vector3(3, 3, 3);
            models.Add(earthModel);

            // Moon model
            moonModel = Content.Load<Model>("planet");
            moonTransform = new Transform();
            moonTransform.Parent = earthTransform;
            moonTransform.LocalScale = new Vector3(1/3f, 1/3f, 1/3f);
            models.Add(moonModel);

            // World Camera
            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = new Vector3(0, 90, 0);
            cameraTransform.Rotate(Vector3.UnitX, -1.55f);
            camera.Transform = cameraTransform;

            // First-person Camera
            playerCamera = new Camera();
            playerCamera.FarPlane = 1000f;
            playerCameraTransform = new Transform();
            playerCameraTransform.LocalPosition = new Vector3(0, 5, 0);
            playerCamera.Transform = playerCameraTransform;
            playerCamera.Transform.Parent = playerTransform;

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

            // Change sim speed
            if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift)) simSpeed += Time.ElapsedGameTime;
            if ((InputManager.IsKeyDown(Keys.LeftControl) || InputManager.IsKeyDown(Keys.RightControl)) && simSpeed > 0) simSpeed -= Time.ElapsedGameTime;

            /// *** Rotation
            // rotate sun
            sunTransform.Rotate(Vector3.Up, simSpeed * Time.ElapsedGameTime);

            // rotate earth
            earthTransform.Rotate(Vector3.Up, simSpeed * -2 * Time.ElapsedGameTime);

            // rotate moon
            moonTransform.Rotate(Vector3.Up, simSpeed * 6 * Time.ElapsedGameTime);
            /// *************

            /// *** Orbiting
            mercTransform.LocalPosition = new Vector3(25 * (float)Math.Sin(simSpeed * 2 *orbitAngle), 30f, 25 * (float)Math.Cos(simSpeed * 2 *orbitAngle));
            earthTransform.LocalPosition = new Vector3(50 * (float)Math.Sin(simSpeed * (orbitAngle + 2.5f)), 30f, 50 * (float)Math.Cos(simSpeed * (orbitAngle + 2.5f)));
            moonTransform.LocalPosition = new Vector3(5 * (float)Math.Sin(simSpeed * 4 *orbitAngle), 0f, 5 * (float)Math.Cos(simSpeed * 4 *orbitAngle));
            orbitAngle += Time.ElapsedGameTime;
            /// ***************

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
            if(InputManager.IsMouseHeld(0))
            {
                playerCameraTransform.LocalPosition += playerTransform.Forward * 10 * Time.ElapsedGameTime;
                playerTransform.LocalPosition += playerTransform.Forward * 10 * Time.ElapsedGameTime;
            }
            if (InputManager.IsMouseHeld(1))
            {
                playerCameraTransform.LocalPosition += playerTransform.Backward * 10 * Time.ElapsedGameTime;
                playerTransform.LocalPosition += playerTransform.Backward * 10 * Time.ElapsedGameTime;
            }

            // rotate the player
            if (InputManager.IsKeyDown(Keys.Left)) playerTransform.Rotate(Vector3.UnitY, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Right)) playerTransform.Rotate(Vector3.UnitY, -Time.ElapsedGameTime);

            // Rotate the camera
            if (InputManager.IsKeyDown(Keys.Up)) playerCameraTransform.Rotate(Vector3.UnitX, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Down)) playerCameraTransform.Rotate(Vector3.UnitX, -Time.ElapsedGameTime);

            // Mouse controls
            if (isFPS)
            {
                if (InputManager.GetMousePosition().X < 360) playerTransform.Rotate(Vector3.UnitY, Time.ElapsedGameTime);
                if (InputManager.GetMousePosition().X > 440) playerTransform.Rotate(Vector3.UnitY, -Time.ElapsedGameTime);

                if (InputManager.GetMousePosition().Y < 200) playerCameraTransform.Rotate(Vector3.UnitX, .5f * Time.ElapsedGameTime);
                if (InputManager.GetMousePosition().Y > 280) playerCameraTransform.Rotate(Vector3.UnitX, -.5f * Time.ElapsedGameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState(); // Fixes model clipping

            Camera cameraInUse;
            if (isFPS) cameraInUse = playerCamera;
            else cameraInUse = camera;

            if (!isFPS) playerModel.Draw(playerTransform.World, cameraInUse.View, cameraInUse.Projection);
            planeModel.Draw(planeTransform.World, cameraInUse.View, cameraInUse.Projection);
            sunModel.Draw(sunTransform.World, cameraInUse.View, cameraInUse.Projection);
            mercModel.Draw(mercTransform.World, cameraInUse.View, cameraInUse.Projection);
            earthModel.Draw(earthTransform.World, cameraInUse.View, cameraInUse.Projection);
            moonModel.Draw(moonTransform.World, cameraInUse.View, cameraInUse.Projection);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Zoom: PAGE UP/DOWN", new Vector2(5, 10), Color.Black);
            _spriteBatch.DrawString(font, "Change Camera: TAB", new Vector2(5, 30), Color.Black);
            _spriteBatch.DrawString(font, "Player Position: WASD", new Vector2(5, 50), Color.Black);
            _spriteBatch.DrawString(font, "Player Rotation: Arrow Keys", new Vector2(5, 70), Color.Black);
            _spriteBatch.DrawString(font, "Player Movement: Hold Mouse Buttons", new Vector2(5, 90), Color.Black);
            _spriteBatch.DrawString(font, "Simulation Speed: SHIFT/CTRL", new Vector2(5, 110), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}