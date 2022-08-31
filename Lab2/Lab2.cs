using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace CPI311.Labs
{
    public class Lab2 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        // Lab2 New Items
        Sprite sprite;
        KeyboardState prevState;

        public Lab2()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //prevState = Keyboard.GetState();
            InputManager.Initialize();
            Time.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            sprite = new Sprite(Content.Load<Texture2D>("Square"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.Space)) sprite.Rotation += 0.05f;
            if (InputManager.IsKeyDown(Keys.Left)) sprite.Position -= Vector2.UnitX * 200 * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Right)) sprite.Position += Vector2.UnitX * 200 * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Up)) sprite.Position -= Vector2.UnitY * 200 * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Down)) sprite.Position += Vector2.UnitY * 200 * Time.ElapsedGameTime;

            // Keyboard input
            //KeyboardState currentState = Keyboard.GetState();
            //if (currentState.IsKeyDown(Keys.Right) && prevState.IsKeyUp(Keys.Left)) sprite.Position += Vector2.UnitX * 5;
            //if (currentState.IsKeyDown(Keys.Left) && prevState.IsKeyUp(Keys.Right)) sprite.Position -= Vector2.UnitX * 5;
            //if (currentState.IsKeyDown(Keys.Up) && prevState.IsKeyUp(Keys.Down)) sprite.Position -= Vector2.UnitY * 5;
            //if (currentState.IsKeyDown(Keys.Down) && prevState.IsKeyUp(Keys.Up)) sprite.Position += Vector2.UnitY * 5;
            //
            // ***** Important to update ******
            //prevState = currentState;
            // ********************************

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            sprite.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}