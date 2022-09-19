using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System.Net.Mime;

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

        Random random = new Random();

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

            bonusSprite = new Sprite(Content.Load<Texture2D>("Square"));
            bonusSprite.Position = new Vector2(random.Next(bonusSprite.Texture.Width, GraphicsDevice.Viewport.Width - bonusSprite.Texture.Width),
                                               random.Next(bonusSprite.Texture.Height, GraphicsDevice.Viewport.Height - bonusSprite.Texture.Height));

            timeBar = new ProgressBar(Content.Load<Texture2D>("Square"));
            timeBar.FillColor = Color.Red;
            timeBar.Scale = new Vector2(3.0f, 1.0f);
            timeBar.Position = new Vector2(50, 25);

            //distanceBar = new ProgressBar(Content.Load<Texture2D>("Square"));
            //distanceBar.Position = new Vector2(70, 20);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            timeBar.Value += Time.ElapsedGameTime;

            if(Vector2.Distance(characterSprite.Position, bonusSprite.Position) < characterSprite.Source.Width)
            {
                timeBar.Value -= 2;
                bonusSprite.Position = new Vector2(random.Next(bonusSprite.Texture.Width, GraphicsDevice.Viewport.Width - bonusSprite.Texture.Width), 
                                                   random.Next(bonusSprite.Texture.Height, GraphicsDevice.Viewport.Height - bonusSprite.Texture.Height));
                bonusSprite.Update();
            }

            // Character moving up
            if (InputManager.IsKeyDown(Keys.Up))
            {
                characterSprite.Position -= 100 * Vector2.UnitY * Time.ElapsedGameTime;
                characterSprite.Source = new Rectangle(0, 128, 32, 32);
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

            bonusSprite.Draw(_spriteBatch);
            characterSprite.Draw(_spriteBatch);

            timeBar.Draw(_spriteBatch);
            //distanceBar.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}