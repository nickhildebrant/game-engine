using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class SineSpiral
    {
        public Sprite Sprite { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public float Speed { get; set; }
        public float Amplitude { get; set; }
        public float Phase { get; set; }
        public float Frequency { get; set; }

        public SineSpiral(Texture2D texture, Vector2 position, float radius = 150, float speed = 1, float amplitude = 10, float phase = 0, float frequency = 20)
        {
            Sprite = new Sprite(texture);
            Position = position;
            Radius = radius;
            Speed = speed;
            Amplitude = amplitude;
            Phase = phase;
            Frequency = frequency;

            Sprite.Position = Position + new Vector2(Radius, 0);
        }

        public void Update()
        {
            Position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Phase += Speed * Time.ElapsedGameTime;

            // Changing the Radius of the main circle
            if (InputManager.IsKeyDown(Keys.Up)) Radius += Time.ElapsedGameTime * 50;
            if (InputManager.IsKeyDown(Keys.Down) && Radius > 0) Radius -= Time.ElapsedGameTime * 50;
            
            // Changing the speed the sprite travels
            if (InputManager.IsKeyDown(Keys.Left) && Speed > 0) Speed -= Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Right)) Speed += Time.ElapsedGameTime;

            // Changing the frequency of the sine wave
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.Right)) Frequency += Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.Left) && Frequency > 0) Frequency -= Time.ElapsedGameTime;

            Sprite.Position = Position + new Vector2(((Radius + Amplitude * (float)Math.Cos(Frequency * Phase)) * (float)Math.Cos(Phase)),
                                                    ((Radius + Amplitude * (float)Math.Cos(Frequency * Phase)) * (float)Math.Sin(Phase)));
        }

        public void Draw(SpriteBatch spriteBatch) { Sprite.Draw(spriteBatch); }
    }
}