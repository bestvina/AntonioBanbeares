using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Antonio
{
    public class Hawk: Enemy
    {
        public Animation FlyingAnimation;
        public Texture2D ShadowTexture;

        public int Health;
        public int Damage;

        public bool Hit;
        public bool Attacking;
        public TimeSpan hitTime;
        public TimeSpan previousHitTime;

        public int Width
        {
            get { return FlyingAnimation.FrameWidth; }
        }

        public int Height
        {
            get { return FlyingAnimation.FrameHeight; }
        }

        float enemyMoveSpeed;

        public void Initialize(Animation flyingAnimation, Texture2D shadowTexture, Vector2 position)
        {
            //load graphics
            FlyingAnimation = flyingAnimation;
            ShadowTexture = shadowTexture;

            Position = position;
            previousPosition = position;

            Active = true;
            Hit = false;
            Attacking = false;

            airborneEnemy = true;
            onGround = false;

            Health = 10;
            Damage = 1;

            //start 100 in air
            ZAxis = 100;

            previousHitTime = TimeSpan.Zero;
            hitTime = TimeSpan.FromSeconds(0.3f);

            enemyMoveSpeed = 1f;
            counter = 0;

        }

        public override void Update(GameTime gametime, List<Bear> bears)
        {
            previousPosition = Position;

            if (this.Hit)
            {
                //ADD NEW IF STATEMENT
                this.Hit = false;
                if (this.Health <= 0)
                {
                    this.Active = false;
                }
            }

            else if (counter == 5) //only adjust movement every 25 executions
            {
                // reset the counter
                counter = 0;

                Vector2 bear0Pos = bears[0].Position;
                Vector2 bear1Pos;
                bool bear0Active = bears[0].Active;
                bool bear1Active;

                if (bears.Count == 2)
                {
                    bear1Pos = bears[1].Position;
                    bear1Active = bears[1].Active;
                }
                else
                {
                    bear1Pos = Vector2.Zero;
                    bear1Active = false;
                }

                //figure out which bear is closer so we can move toward him
                Vector2 closerPosition;
                float smallerXDist;
                float antonioXDist = Math.Abs(Position.X - bear0Pos.X);
                float joseXDist = Math.Abs(Position.X - bear1Pos.X);

                if (!bear1Active)
                {
                    closerPosition = bear0Pos;
                    smallerXDist = antonioXDist;
                }
                else if (!bear0Active)
                {
                    closerPosition = bear1Pos;
                    smallerXDist = joseXDist;
                }
                else if (antonioXDist < joseXDist)
                {
                    closerPosition = bear0Pos;
                    smallerXDist = antonioXDist;
                }
                else
                {
                    closerPosition = bear1Pos;
                    smallerXDist = joseXDist;
                }

                //now figure out where exactly cactus is going
                if (closerPosition.X > Position.X)
                {
                    moveRight = 1;
                    FlyingAnimation.FacingRight = true;
                }
                else if (closerPosition.X < Position.X)
                {
                    moveRight = -1; //move left
                    FlyingAnimation.FacingRight = false;
                }
                //Don't move if enemy is on player
                else moveRight = 0;

                //Only move ememy on y axis if near a player
                if (smallerXDist < 100)
                {

                    if (closerPosition.Y > Position.Y)
                    {
                        moveUp = 1;
                    }
                    else if (closerPosition.Y < Position.Y)
                    {
                        moveUp = -1; //move down
                    }
                    else moveUp = 0;
                    // Move the enemy toward the closer bear. 

                }
                else moveUp = 0;
            }

            //actually alter the position
            Position.X += (enemyMoveSpeed * moveRight);
            Position.Y += (enemyMoveSpeed * moveUp);
            counter++;

            // Update the position of the Animation
            FlyingAnimation.Position = Position;

            // Update Animation
            FlyingAnimation.Update(gametime);
            

            //Collision Detection!!!
            Rectangle rectangle1;
            Rectangle rectangle2;

            foreach (Bear bear in bears) //THIS IS COLLISION FOR PUNCHING
            {
                //If bear isn't punching, just move to the next bear
                if (!bear.Attacking || bear.ZAxis < this.ZAxis - (this.Height / 2) || bear.ZAxis > this.ZAxis + this.Height / 2)
                {
                    continue;
                }

                int fistX;// figure out where the bear's fist is. If facing right, it's to the right and vice versa
                if (bear.FacingRight)
                {
                    fistX = (int)bear.Position.X + (bear.Width / 4);
                }
                else fistX = (int)bear.Position.X - (3 * bear.Width / 4);
                // can punch  25 up or down and half the width, starting a quarter from center
                rectangle1 = new Rectangle(fistX, (int)bear.Position.Y - 25, bear.Width / 2, 50);

               
                if (this.Hit)//if the enemy is hit. move to the next one
                {
                    continue;
                }

                rectangle2 = new Rectangle((int)this.Position.X - (this.Width / 4), (int)this.Position.Y - 50, this.Width / 2, 50);

                //Determine if overlapping 
                if (rectangle1.Intersects(rectangle2))
                {
                    this.Health -= bear.PunchDamage;
                    this.Hit = true;
                    this.previousHitTime = gametime.TotalGameTime;

                }  
            }
            // COLLISION FOR CACTII STABBING BEARS
            // Should be the middle half of bear & cactus and height 20 with bear at height = 10
            if (!this.Hit)
            {

                rectangle1 = new Rectangle((int)this.Position.X - (this.Width / 4), (int)this.Position.Y - 10, this.Width / 2, 20);

                foreach (Bear bear in bears)
                {
                    if (!bear.Active || bear.ZAxis < this.ZAxis - this.Height / 2 || bear.ZAxis > this.ZAxis + this.Height / 2)
                    {
                        continue;
                    }
                    rectangle2 = new Rectangle((int)bear.Position.X - (bear.Width / 4), (int)bear.Position.Y - 10, bear.Width / 2, 20);
                    if (rectangle1.Intersects(rectangle2))
                    {
                        if (!bear.Invincible) // Bear can't get hit twice but we still want the cactii to attack even if no damage
                        {
                            bear.Hit = true;
                            bear.Invincible = true;
                            bear.Health -= this.Damage;
                            bear.previousHitTime = gametime.TotalGameTime;
                        }
                        if (!this.Attacking) //This makes it so the cactus doesn't re-attack if another bear touches him. Bear just gets hurt
                        {
                            this.Attacking = true;
                            //this.previousAttackTime = gametime.TotalGameTime;
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, int xOffset)
        {
            //figure out where the bear is drawn on screen based on position and how much the level has scrolled
            Vector2 PositionOnScreen = new Vector2(Position.X - xOffset, Position.Y - ZAxis);

            //Draw the bear's shadow
            spriteBatch.Draw(ShadowTexture, new Vector2(Position.X - xOffset, Position.Y + Height - ShadowTexture.Height - groundHeight), null, Color.White, 0f, new Vector2(Width / 2, this.Height), 1f, SpriteEffects.None, 0f);

            // Draw the animation
            FlyingAnimation.Draw(spriteBatch, PositionOnScreen);
        
        }

    }
}
