using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Antonio
{
    class Background
    {
        // The image representing the parallaxing background
        Texture2D texture;

        // An array of positions of the parallaxing background
        Vector2[] positions;

        public int xPosition;
        
        //How far down from the top should this background get drawn;
        public int height;

        // The speed which the background is moving
        float speed;
        
        public void Initialize(Texture2D texture, int height, int screenWidth, float speed)
        {
            this.texture = texture;

            // Set the speed of the background
            this.speed = speed;

            this.height = height;

            // If we divide the screen with the texture width then we can determine the number of tiles need.
            // We add 1 to it so that we won't have a gap in the tiling
            positions = new Vector2[2];

            xPosition = 0;

            // Set the initial positions of the parallaxing background
            for (int i = 0; i < positions.Length; i++)
            {
                // We need the tiles to be side by side to create a tiling effect
                positions[i] = new Vector2(i * texture.Width, this.height);
            }
        }
        public void Update(bool scrolling)
        {
            
            //only update the background if it's scrolling
            if (scrolling){
                // Update the positions of the background
                for (int i = 0; i < positions.Length; i++)
                {
                    // Update the position of the screen by adding the speed
                    positions[i].X += speed;
                    // If the speed has the background moving to the left
                    if (speed <= 0)
                    {
                        // Check the texture is out of view then put that texture at the end of the screen
                        if (positions[i].X <= -texture.Width)
                        {
                            positions[i].X = texture.Width * (positions.Length - 1);
                        }
                    }

                    // If the speed has the background moving to the right
                    else
                    {
                        // Check if the texture is out of view then position it to the start of the screen
                        if (positions[i].X >= texture.Width * (positions.Length - 1))
                        {
                            positions[i].X = -texture.Width;
                        }
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                spriteBatch.Draw(texture, positions[i], Color.White);
            }
        }
    }
}
