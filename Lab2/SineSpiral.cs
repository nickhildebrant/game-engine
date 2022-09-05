using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI311.GameEngine;

namespace CPI311.Labs
{
    public class SineSpiral
    {
        public Sprite Sprite { get; set; }
        public Vector2 Origin { get; set; }
        public float Radius { get; set; }
        public float Speed { get; set; }
        public float Amplitude { get; set; }
        public float Period { get; set; }
        public float TimePassed { get; set; }

        public SineSpiral(Texture2D texture, Vector2 pos)
        {
            Sprite = new Sprite(texture);
            Origin = pos;
            Radius = 150f;
            Speed = 1f;

            Amplitude = 20f;
            Period = 15f;
            TimePassed = 0f;
        }

        public virtual void Update()
        {
            // x = (r+Acos(t))*cos(t), y = (r+Acos(t))*sin(t)
            TimePassed += Speed * Time.ElapsedGameTime;
            Sprite.Position = Origin + new Vector2(((Radius + Amplitude * (float)Math.Cos(Period * TimePassed)) * (float)Math.Cos(TimePassed)),
                                                   ((Radius + Amplitude * (float)Math.Cos(Period * TimePassed)) * (float)Math.Sin(TimePassed)));
        }

        public virtual void Draw(SpriteBatch spriteBatch) { Sprite.Draw(spriteBatch); }
    }
}