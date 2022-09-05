using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CPI311.GameEngine;

namespace CPI311.Labs
{
    public class SineSpiral
    {
        public Sprite Sprite { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public float Speed { get; set; }
        public float Amplitude { get; set; }
        public float Period { get; set; }
        public float Phase { get; set; }

        public SineSpiral(Texture2D texture, Vector2 position)
        {
            Sprite = new Sprite(texture);
            Position = position;
            Radius = 150f;
            Speed = 1f;

            Amplitude = 20f; // A -- Amplitude
            Period = 20f;    // B -- Frequency, tied to period
            Phase = 0f;      // C -- Phase, horizontal shift
        }

        public void Update()
        {
            // y = Asin(Bx+C)+D -- Sine graph
            Phase += Speed * Time.ElapsedGameTime;
            Sprite.Position = Position + new Vector2((float)((Radius + Amplitude * Math.Cos(Period * Phase)) * Math.Cos(Phase)),
                                                     (float)((Radius + Amplitude * Math.Cos(Period * Phase)) * Math.Sin(Phase)));
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch);
        }
    }
}