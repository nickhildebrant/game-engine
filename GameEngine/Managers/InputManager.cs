using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public static class InputManager
    {
        static KeyboardState PreviousKeyboardState { get; set; }
        static KeyboardState CurrentKeyboardState { get; set; }
        static MouseState PreviousMouseState { get; set; }
        static MouseState CurrentMouseState { get; set; }

        public static void Initialize()
        {
            PreviousKeyboardState = CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState = Mouse.GetState();
        }

        public static void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public static bool IsKeyDown(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) &&
                PreviousKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if a mouse button is clicked 0:Left, 1:Right, 2:Middle
        /// </summary>
        public static bool IsMouseClicked(int mouseButton)
        {
            switch(mouseButton)
            {
                case 0:
                    return CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton != ButtonState.Pressed;
                    
                case 1:
                    return CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton != ButtonState.Pressed;

                case 2:
                    return CurrentMouseState.MiddleButton == ButtonState.Pressed && PreviousMouseState.MiddleButton != ButtonState.Pressed;
            }

            return false;
        }

        /// <summary>
        /// Checks if a mouse button is down 0:Left, 1:Right, 2:Middle
        /// </summary>
        public static bool IsMouseHeld(int mouseButton)
        {
            switch (mouseButton)
            {
                case 0:
                    return CurrentMouseState.LeftButton == ButtonState.Pressed;

                case 1:
                    return CurrentMouseState.RightButton == ButtonState.Pressed;

                case 2:
                    return CurrentMouseState.MiddleButton == ButtonState.Pressed;
            }

            return false;
        }

        /// <summary>
        /// Checks if a mouse button has been released 0:Left, 1:Right, 2:Middle
        /// </summary>
        public static bool IsMouseReleased(int mouseButton)
        {
            switch (mouseButton)
            {
                case 0:
                    return CurrentMouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Pressed;

                case 1:
                    return CurrentMouseState.RightButton == ButtonState.Released && PreviousMouseState.RightButton == ButtonState.Pressed;

                case 2:
                    return CurrentMouseState.MiddleButton == ButtonState.Released && PreviousMouseState.MiddleButton == ButtonState.Pressed;
            }

            return false;
        }

        public static Vector2 GetMousePosition() { return new Vector2(CurrentMouseState.X, CurrentMouseState.Y); }
    }
}
