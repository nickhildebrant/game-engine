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

        public static bool IsMouseLeftClicked()
        { 
            return CurrentMouseState.LeftButton == ButtonState.Pressed
                && PreviousMouseState.LeftButton != ButtonState.Pressed; 
        }

        public static bool IsMouseRightClicked()
        {
            return CurrentMouseState.RightButton == ButtonState.Pressed
                && PreviousMouseState.RightButton != ButtonState.Pressed;
        }

        public static bool IsMouseLeftDown()
        {
            return CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsMouseRightDown()
        {
            return CurrentMouseState.RightButton == ButtonState.Pressed;
        }

        public static Vector2 GetMousePosition() { return new Vector2(CurrentMouseState.X, CurrentMouseState.Y); }
    }
}
