using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class AnimatedSprite : Sprite
    {
        public int Frames { get; set; }
        public float Frame { get; set; }
        public float Speed { get; set; }

        public AnimatedSprite(Texture2D texture, int frames = 1) : base (texture)
        {
            Frames = frames;
            Frame = 0;
            Speed = 5;
        }

        public override void Update()
        {
            Frame += Speed * Time.ElapsedGameTime;

            if (Frame >= Frames) Frame = 0;

            int frameNum = (int)Math.Ceiling((double)Frame);
            Source = new Rectangle(frameNum * 32, Source.Y, 32, 32);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, Effect, Layer);
        }
    }
}
