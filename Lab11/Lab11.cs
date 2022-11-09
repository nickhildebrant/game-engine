using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System.Collections.Generic;

namespace CPI311.Labs
{
    public class Lab11 : Game
    {
        // **** inner class *****
        public class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }
        // ***************************

        Dictionary<string, Scene> scenes;
        Scene currentScene;
        List<GUIElement> guiElements;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D texture;
        SpriteFont font;
        Color background = Color.CornflowerBlue;

        public Lab11()
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

            scenes = new Dictionary<string, Scene>();
            guiElements = new List<GUIElement>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>("Square");

            font = Content.Load<SpriteFont>("font");

            //Button exitButton = new Button();
            //exitButton.Texture = texture;
            //exitButton.Text = "Exit";
            //exitButton.Bounds = new Rectangle(50, 50, 300, 20);
            //exitButton.Action += ExitGame;
            //guiElements.Add(exitButton);

            Checkbox box = new Checkbox();
            box.Texture = texture;
            box.Box = texture;
            box.Text = "Switch Scene";
            box.Bounds = new Rectangle(50, 50, 300, 50);
            box.Action += SwitchScenes;
            guiElements.Add(box);

            Button fullButton = new Button();
            fullButton.Text = "Fullscreen Mode";
            fullButton.Texture = texture;
            fullButton.Bounds = new Rectangle(50, 200, 300, 10);
            fullButton.Action += FullScreen;
            guiElements.Add(fullButton);

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            currentScene = scenes["Menu"];
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            InputManager.Update();

            currentScene.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            //_spriteBatch.Begin();
            //exitButton.Draw(_spriteBatch, font);
            //_spriteBatch.End();

            currentScene.Draw();

            base.Draw(gameTime);
        }

        void SwitchScenes(GUIElement element)
        {
            currentScene = (currentScene == scenes["Play"] ? scenes["Menu"] : scenes["Play"]);
        }

        void FullScreen(GUIElement element)
        {
            //ScreenManager.Setup(1920, 1080);
            ScreenManager.IsFullScreen = !ScreenManager.IsFullScreen;
        }

        void ExitGame(GUIElement element)
        {
            currentScene = scenes["Play"];
            background = (background == Color.White ? Color.Blue : Color.White);
        }

        void MainMenuUpdate()
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }

        void MainMenuDraw()
        {
            _spriteBatch.Begin();
            foreach (GUIElement element in guiElements) element.Draw(_spriteBatch, font);
            _spriteBatch.End();
        }

        void PlayUpdate()
        {
            if (InputManager.IsKeyReleased(Keys.Escape)) currentScene = scenes["Menu"];
        }

        void PlayDraw()
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Play Mode! Press \"Esc\" to go back", Vector2.Zero, Color.Black);
            _spriteBatch.End();
        }
    }
}