using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine
{
    public class Sprite
    {
        // Constructor
        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Position = new Microsoft.Xna.Framework.Vector2(0, 0);
            Source = new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height);
            Color = Microsoft.Xna.Framework.Color.White;
            Rotation = 0;
            Origin = new Microsoft.Xna.Framework.Vector2(Texture.Width / 2, Texture.Height / 2);
            Scale = new Microsoft.Xna.Framework.Vector2(1, 1);
            Effect = SpriteEffects.None;
            Layer = 1;
        }

        // Properties
        public Texture2D Texture { get; set; }
        public Microsoft.Xna.Framework.Vector2 Position { get; set; }
        public Microsoft.Xna.Framework.Rectangle Source { get; set; }
        public Microsoft.Xna.Framework.Color Color { get; set; }
        public Single Rotation { get; set; }
        public Microsoft.Xna.Framework.Vector2 Origin { get; set; }
        public Microsoft.Xna.Framework.Vector2 Scale { get; set; }
        public SpriteEffects Effect { get; set; }
        public Single Layer { get; set; }

        // Update and Draw
        public virtual void Update() { }
        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, Effect, Layer);
        }
    }
}
