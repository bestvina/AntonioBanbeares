using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Antonio
{
    class Box: DrawObject
    {
        Texture2D BoxTexture;
        public bool Active;
        
        //Whether or not something is on top of the object
        public bool inStack;
        
        //Whether the object should harm the player if they step on it ie spikes, lava, etc!
        public bool deadly;

        //what portion of the box's frame is depth. If the box is flat, 100% of the frame is teh depth. If half box is as tall as it is deep, 50% of the frame is depth.
        public float DepthPortion;
        public float ExtraPercentage; //Extra size added to rectangle to make hit detection more leniant/look better

        Random random;

        public int Width
        {
            get { return (int) (BoxTexture.Width); }
        }

        //depth - how far from the front to the back of the box; note that the pseudo-3d nature of the game. The depth is not the height of the frame
        public int Depth
        {
            get { return (int)(BoxTexture.Height * DepthPortion); }
        }

        // Get the height (this is the depth of the box as well of the box
        public int Height
        {
            get { return (int) (BoxTexture.Height * (1 - DepthPortion)); }
        }

        public int HowTall
        {
            get { return (int)(Height + ZAxis); }
        }

        public Box (Texture2D boxTexture, Vector2 position, float zaxis, bool inStack, float depthPortion, float extraPercentage, bool deadly )
        {
            // 
            BoxTexture = boxTexture;
            Position = position;
            ZAxis = zaxis;
            Active = true;
            this.inStack = inStack;
            this.DepthPortion = depthPortion;
            ExtraPercentage = extraPercentage;
            this.deadly = deadly;
            random = new Random();
        }

        public void Update(List<Bear> bears, List<Enemy> enemies, GameTime gameTime)
        {
            Rectangle rect = new Rectangle((int)(this.Position.X - this.Width * ExtraPercentage), (int)(this.Position.Y - this.Depth * ExtraPercentage), (int)(this.Width * (1 + 2 * ExtraPercentage)), (int)(this.Depth * (1 + 2 * ExtraPercentage)) + 1);

            foreach (Bear bear in bears)
            {
                if (rect.Contains(new Point((int)bear.Position.X, (int)bear.Position.Y)))
                {
                    if (bear.ZAxis >= this.HowTall && !this.inStack)
                    {
                        if (this.HowTall > bear.groundHeight) bear.groundHeight = this.HowTall; //make sure the tallest object under bear is where shadow is
                    }
                    else if ((bear.ZAxis >= (this.HowTall - (bear.Height * .2)))
                              && (bear.ZAxis <= this.HowTall)
                              && bear.inAir && (bear.velocity < 0) && !this.inStack)// land on box if falling and in top portion of box
                    {
                        bear.ZAxis = this.HowTall;
                        bear.inAir = false;
                        bear.velocity = 0;
                        bear.groundHeight = this.HowTall;

                    }
                    //else if (bear.ZAxis == this.HowTall && !bear.inAir)
                    //{
                    //  bear.groundHeight = this.Height;
                    //}
                    else if (bear.ZAxis < this.HowTall) //make sure bear doesn't walk thru box.
                    {
                        //The first two if's allow the bear to skirt along the edge of the box
                        if (!rect.Contains(new Point((int)bear.Position.X, (int)bear.previousPosition.Y)))
                        {
                            bear.Position = new Vector2((int)bear.Position.X, (int)bear.previousPosition.Y);
                        }
                        else if (!rect.Contains(new Point((int)bear.previousPosition.X, (int)bear.Position.Y)))
                        {
                            bear.Position = new Vector2((int)bear.previousPosition.X, (int)bear.Position.Y);
                        }
                        else bear.Position = bear.previousPosition;
                    }
                    
                    //for deadly stuff O.O
                    if (this.deadly && !bear.Invincible && bear.ZAxis >= this.ZAxis - 20 && bear.ZAxis <= this.ZAxis + 20)//for spikes, lava, etc.
                    {
                        bear.Hit = true;
                        bear.Invincible = true;
                        bear.Health -= 1;
                        bear.previousHitTime = gameTime.TotalGameTime;
                    }
                }

            }
            foreach (Enemy enemy in enemies)
            {
                if (rect.Contains(new Point((int)enemy.Position.X, (int)enemy.Position.Y)))
                {
                    if (enemy.ZAxis < this.HowTall) //make sure enemy doesn't move thru box.
                    {
                        //want enemy to get far away from box so pick a new direction and start new movement cycle(counter = 0)
                        enemy.counter = 0;

                        //For if the enemy rams directly into the object go perpendicular
                        if (enemy.moveRight == 0)
                        {
                            if (enemy.moveRight == random.Next(0, 2))
                            {
                                enemy.moveRight = 1;
                            }
                            else enemy.moveRight = -1;
                        }
                        else if (enemy.moveUp == 0)
                        {
                            if (enemy.moveUp == random.Next(0, 2))
                            {
                                enemy.moveUp = 1;
                            }
                            else enemy.moveUp = -1;
                        }
                        //from here figure out whether it was the x or y movement that pushed the enemy into the box
                        else if (!rect.Contains(new Point((int)enemy.Position.X, (int)enemy.previousPosition.Y)))
                        {
                            //the Y movement pushed the enemy into the box so reverse the y direction and start movement over. 
                            //Example - if the enemy moves up-right-diagonal into the left side of a box, they should bounce off
                            //        - perpendicular and go up-left
                            enemy.Position = new Vector2((int)enemy.Position.X, (int)enemy.previousPosition.Y);
                            enemy.moveUp *= -1;
                        }
                        else if (!rect.Contains(new Point((int)enemy.previousPosition.X, (int)enemy.Position.Y)))
                        {
                            //The x movement pushed the enemy into the box so reverse the x
                            enemy.Position = new Vector2((int)enemy.previousPosition.X, (int)enemy.Position.Y);
                            enemy.moveRight *= -1;
                        }
                        else //if enemy perfectly rams a diagonal of a box they would get stuck without this
                        {
                            enemy.Position = enemy.previousPosition;
                            enemy.moveRight *= -1;
                            enemy.moveUp *= -1;
                        }


                    }
                    else { 
                        enemy.onGround = true;
                        enemy.groundHeight = this.HowTall;            
                    } //if the enemy isn't inside an object they're on it
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch, int xOffset)
        {
            //draw the cactus being hit!
            if (Active)
            {
                //figure out where the bear is drawn on screen based on position and how much the level has scrolled
                Vector2 PositionOnScreen = new Vector2(Position.X - xOffset, Position.Y - ZAxis);

                // Draw from top left
                spriteBatch.Draw(BoxTexture, PositionOnScreen, null, Color.White, 0f, new Vector2(0, this.Height), 1f, SpriteEffects.None, 0f); 
            }
        }
    }
}
