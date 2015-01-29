using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Antonio
{
    class Taco: DrawObject
    {
        Texture2D TacoTexture;
        public bool active;

        public Taco(Texture2D tacoTexture, Vector2 position, int zaxis)
        {
            // Load the enemy ship texture
            TacoTexture = tacoTexture;
            Position = position;
            active = true;
            ZAxis = zaxis;
        }

        public int Width
        {
            get { return TacoTexture.Width; }
        }

        // Get the height of the bear
        public int Height
        {
            get { return TacoTexture.Height; }
        }

        public void Update(List<Bear> bears)
        {
            // Use the Rectangle's built-in intersect function to 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            if (this.active) //If there's a taco on teh screen, see if a bear grabs it
            {
                rectangle1 = new Rectangle((int)this.Position.X - (this.Width / 4), (int)this.Position.Y - (this.Height / 4), this.Width / 2, this.Height / 2);
                foreach (Bear bear in bears)
                {
                    if (!bear.Active || bear.inAir || (bear.ZAxis != this.ZAxis))
                    {
                        continue;
                    }
                    rectangle2 = new Rectangle((int)bear.Position.X - (bear.Width / 4), (int)bear.Position.Y - 20, bear.Width / 2, 40);
                    if (rectangle1.Intersects(rectangle2))
                    {
                        this.active = false;
                        bear.Health++;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, int xOffset)
        {
            //draw the cactus being hit!
            if (active)
            {
                //figure out where the taco is drawn on screen based on position and how much the level has scrolled and how hi in the air it is
                Vector2 PositionOnScreen = new Vector2(Position.X - xOffset, Position.Y - ZAxis);

                spriteBatch.Draw(TacoTexture, PositionOnScreen, null, Color.White, 0f, new Vector2(Width / 2, Height), 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
