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
        SineSpiral sprite;

        public Lab2()
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
            sprite = new SineSpiral(Content.Load<Texture2D>("Square"), new Vector2(GraphicsDevice.Viewport.Bounds.Width/2, GraphicsDevice.Viewport.Bounds.Height/2));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);
            sprite.Update();

            //if (InputManager.IsKeyDown(Keys.Left) && sprite.Radius > 0) sprite.Radius -= Time.ElapsedGameTime * 100;
            //if (InputManager.IsKeyDown(Keys.Right)) sprite.Radius += Time.ElapsedGameTime * 100;
            //if (InputManager.IsKeyDown(Keys.Up)) sprite.Speed += Time.ElapsedGameTime;
            //if (InputManager.IsKeyDown(Keys.Down) && sprite.Speed > 0) sprite.Speed -= Time.ElapsedGameTime;

            //if (InputManager.IsKeyDown(Keys.Space)) sprite.Rotation += 0.05f;
            //if (InputManager.IsKeyDown(Keys.Left)) sprite.Position -= Vector2.UnitX * 20 * Time.ElapsedGameTime;
            //if (InputManager.IsKeyDown(Keys.Right)) sprite.Position += Vector2.UnitX * 20 * Time.ElapsedGameTime;
            //if (InputManager.IsKeyDown(Keys.Up)) sprite.Position -= Vector2.UnitY * 20 * Time.ElapsedGameTime;
            //if (InputManager.IsKeyDown(Keys.Down)) sprite.Position += Vector2.UnitY * 20 * Time.ElapsedGameTime;

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