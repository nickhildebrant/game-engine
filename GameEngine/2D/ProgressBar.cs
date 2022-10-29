using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class ProgressBar : Sprite
    {
        public Color FillColor { get; set; }
        public float Value { get; set; }

        public ProgressBar(Texture2D texture) : base(texture)
        {
            Color = Color.DarkGray;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch); // let the sprite do its work
            spriteBatch.Draw(Texture, Position, new Rectangle(0, 0, (int)Value, Texture.Height),
                FillColor, Rotation, Origin, Scale, Effect, Layer);
        }
    }
}
