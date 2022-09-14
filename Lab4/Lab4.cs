using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace CPI311.Labs
{
    public class Lab4 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // *** Lab4
        Model model;
        Transform modelTransform;

        // *** Last part of Lab4
        Model parentModel;
        Transform parentTransform;

        Camera camera;
        Transform cameraTransform;
        // *************************

        public Lab4()
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

            parentModel = Content.Load<Model>("Sphere");
            parentTransform = new Transform();

            model = Content.Load<Model>("Torus");
            modelTransform = new Transform();
            modelTransform.Parent = parentTransform;
            modelTransform.LocalPosition = Vector3.Forward * 2;

            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 5;
            camera.Transform = cameraTransform;

            // ************* Add lighting *******************
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
            }

            foreach (ModelMesh mesh in parentModel.Meshes)
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.W)) cameraTransform.LocalPosition += cameraTransform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.A)) cameraTransform.Rotate(cameraTransform.Up, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.S)) cameraTransform.LocalPosition += cameraTransform.Backward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.D)) cameraTransform.Rotate(cameraTransform.Down, Time.ElapsedGameTime);

            // Controls parent movement
            if (InputManager.IsKeyDown(Keys.Up)) parentTransform.LocalPosition += cameraTransform.Up * Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.Down)) parentTransform.LocalPosition += cameraTransform.Down * Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.Left)) parentTransform.LocalPosition += cameraTransform.Left * Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.Right)) parentTransform.LocalPosition += cameraTransform.Right * Time.ElapsedGameTime * 5;

            // Controls child rotation
            if (InputManager.IsKeyDown(Keys.Z)) parentTransform.Rotate(cameraTransform.Up, Time.ElapsedGameTime * 5);
            if (InputManager.IsKeyDown(Keys.X)) parentTransform.Rotate(cameraTransform.Down, Time.ElapsedGameTime * 5);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            model.Draw(modelTransform.World, camera.View, camera.Projection);
            parentModel.Draw(parentTransform.World, camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}