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
            characterSprite.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            characterSprite.Source = new Rectangle(0, 32, 32, 32);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            // Character moving up
            if (InputManager.IsKeyDown(Keys.Up))
            {
                characterSprite.Position -= 100 * Vector2.UnitY * Time.ElapsedGameTime;
                characterSprite.Source = new Rectangle(0, 0, 32, 32);
                characterSprite.Update();
            }

            // Character moving down
            if (InputManager.IsKeyDown(Keys.Down))
            {
                characterSprite.Position += 100 * Vector2.UnitY * Time.ElapsedGameTime;
                characterSprite.Source = new Rectangle(0, 32, 32, 32);
                characterSprite.Update();
            }

            // Character moving left
            if (InputManager.IsKeyDown(Keys.Left))
            {
                characterSprite.Position -= 100 * Vector2.UnitX * Time.ElapsedGameTime;
                characterSprite.Source = new Rectangle(0, 64, 32, 32);
                characterSprite.Update();
            }

            // Character moving right
            if (InputManager.IsKeyDown(Keys.Right))
            {
                characterSprite.Position += 100 * Vector2.UnitX * Time.ElapsedGameTime;
                characterSprite.Source = new Rectangle(0, 96, 32, 32);
                characterSprite.Update();
            }

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