using System;
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
        int direction = 1;
        AnimatedSprite characterSprite;
        Sprite bonusSprite;
        SpriteFont font;

        ProgressBar timeBar;
        ProgressBar distanceBar;

        // for mouse movement
        bool isThere = true;
        Vector2 destination;

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

            font = Content.Load<SpriteFont>("font");

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

            distanceBar = new ProgressBar(Content.Load<Texture2D>("Square"));
            distanceBar.FillColor = Color.Green;
            distanceBar.Scale = new Vector2(3.0f, 1.0f);
            distanceBar.Position = new Vector2(300, 25);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            timeBar.Value += Time.ElapsedGameTime;

            if (InputManager.IsKeyPressed(Keys.F)) distanceBar.Value = 28;

            // Collecting the bonus subtracts 2.5 seconds
            if (Vector2.Distance(characterSprite.Position, bonusSprite.Position) < characterSprite.Source.Width)
            {
                timeBar.Value -= 2.5f;
                bonusSprite.Position = new Vector2(random.Next(bonusSprite.Texture.Width, GraphicsDevice.Viewport.Width - bonusSprite.Texture.Width),
                                                    random.Next(bonusSprite.Texture.Height, GraphicsDevice.Viewport.Height - bonusSprite.Texture.Height));
                bonusSprite.Update();
            }

            // When the mouse is clicked
            if (InputManager.IsMouseLeftClicked())
            {
                destination = InputManager.GetMousePosition();
                isThere = false;
            }

            if (Vector2.Distance(characterSprite.Position, destination) < 1.5f) isThere = true;

            if (!isThere && distanceBar.Value <= 31 && timeBar.Value <= 31)
            {
                // Move X first
                if (characterSprite.Position.X > destination.X) { direction = 2; characterSprite.Position -= 100 * Vector2.UnitX * Time.ElapsedGameTime; }
                if (characterSprite.Position.X < destination.X) { direction = 0; characterSprite.Position += 100 * Vector2.UnitX * Time.ElapsedGameTime; }

                // When the x matches, then move Y
                if(Math.Abs(characterSprite.Position.X - destination.X) < 1.5f)
                {
                    if (characterSprite.Position.Y > destination.Y) { direction = 3; characterSprite.Position -= 100 * Vector2.UnitY * Time.ElapsedGameTime; }
                    if (characterSprite.Position.Y < destination.Y) { direction = 1; characterSprite.Position += 100 * Vector2.UnitY * Time.ElapsedGameTime; }
                }

                distanceBar.Value += 0.5f * Time.ElapsedGameTime;
                distanceBar.Update();

                characterSprite.Source = new Rectangle(0, direction * 32, 32, 32);
                characterSprite.Update();
            }

            // Character moving up
            if (InputManager.IsKeyDown(Keys.Up))
            {
                if(direction == 0) characterSprite.Position += 100 * Vector2.UnitX * Time.ElapsedGameTime;
                if(direction == 1) characterSprite.Position += 100 * Vector2.UnitY * Time.ElapsedGameTime;
                if(direction == 2) characterSprite.Position -= 100 * Vector2.UnitX * Time.ElapsedGameTime;
                if(direction == 3) characterSprite.Position -= 100 * Vector2.UnitY * Time.ElapsedGameTime;
                characterSprite.Update();

                distanceBar.Value += 0.5f * Time.ElapsedGameTime;
                distanceBar.Update();
            }

            // Character moving left
            if (InputManager.IsKeyPressed(Keys.Left))
            {
                if (direction > 0) direction--;
                else direction = 3;
                characterSprite.Source = new Rectangle(0, direction * 32, 32, 32);
                characterSprite.Update();
            }

            // Character moving right
            if (InputManager.IsKeyPressed(Keys.Right))
            {
                if (direction < 3) direction++;
                else direction = 0;
                characterSprite.Source = new Rectangle(0, direction * 32, 32, 32);
                characterSprite.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            if (distanceBar.Value <= 31)
            {
                if(timeBar.Value <= 31)
                {
                    bonusSprite.Draw(_spriteBatch);
                    characterSprite.Draw(_spriteBatch);

                    timeBar.Draw(_spriteBatch);
                    distanceBar.Draw(_spriteBatch);

                    _spriteBatch.DrawString(font, "Time Remaining", new Vector2(110, 20), Color.Black);
                    _spriteBatch.DrawString(font, "Distance Traveled", new Vector2(360, 20), Color.Black);
                }
                else _spriteBatch.DrawString(font, "Game Over, Time is Up! Exit or press the escape key.", new Vector2(200, 240), Color.Black);
            }
            else _spriteBatch.DrawString(font, "Game Over, you walked enough distance! Exit or press the escape key.", new Vector2(150, 240), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}