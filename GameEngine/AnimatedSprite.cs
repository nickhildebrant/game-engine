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
            Speed = 1;
        }

        public override void Update()
        {
            Frame += Speed * Time.ElapsedGameTime;

            if (Frame >= Frames) Frame = 0;

            Source = new Rectangle(0, 0, Texture.Width, Texture.Height);
        }
    }
}
