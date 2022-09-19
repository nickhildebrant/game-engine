using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Assignment1
{
    public class Assignment1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // *** Assignment 1
        AnimatedSprite characterSprite;
        Sprite bonusSprite;

        ProgressBar timeBar;
        ProgressBar distanceBar;

        public Assignment1()
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

            characterSprite = new AnimatedSprite(Content.Load<Texture2D>("explorer"), 8);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.Up)) characterSprite.Position -= 100 * Vector2.UnitY * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Down)) characterSprite.Position += 100 * Vector2.UnitY * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Left)) characterSprite.Position -= 100 * Vector2.UnitX * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Right)) characterSprite.Position += 100 * Vector2.UnitX * Time.ElapsedGameTime;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            characterSprite.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}