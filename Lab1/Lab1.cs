using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPI311.Labs {
    public class Lab1 : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SpriteFont font;

        public Lab1() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() { base.Initialize(); }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Hello, World!", new Vector2(100, 100), Color.Black);
            _spriteBatch.DrawString(font, "CPI311 2022", new Vector2(100, 120), Color.Black);
            _spriteBatch.DrawString(font, "Yoshi is the instructor", new Vector2(100, 140), Color.Black);
            _spriteBatch.DrawString(font, "brabrabrba", new Vector2(100, 160), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}