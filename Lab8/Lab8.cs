using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using CPI311.GameEngine;

namespace CPI311.Labs
{
    public class Lab8 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Lab 8 sounds
        SoundEffect gunSound;
        SoundEffectInstance soundInstance;

        Model model;
        Camera camera, topDownCamera;

        Effect effect;

        List<Transform> transforms;
        List<Collider> colliders;
        List<Camera> cameras;

        public Lab8()
        {
            _graphics = new GraphicsDeviceManager(this);
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

            ScreenManager.Setup(true, 1920, 1080);

            gunSound = Content.Load<SoundEffect>("Gun");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            
            Time.Update(gameTime);
            InputManager.Update();

            if (InputManager.IsMouseLeftClicked())
            {
                SoundEffectInstance instance = gunSound.CreateInstance();
                instance.Play();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}