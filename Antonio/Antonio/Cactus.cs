using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Antonio
{
    public class Cactus: Enemy
    {
        // Animation representing the enemy
        public Animation WalkingAnimation;
        public Texture2D HitTexture;
        public Texture2D AttackingTexture;

        // The position of the enemy ship relative to the top left corner of thescreen
        //public Vector2 Position;

        //Whether the cactus is green or red
        public bool Green;

        // The hit points of the enemy, if this goes to zero the enemy dies
        public int Health;

        // The amount of damage the enemy inflicts on the player ship
        public int Damage;

        // The amount of score the enemy will give to the player
        public int Value;

        //time logic for when enemy gets hit
        public bool Hit;
        public TimeSpan previousHitTime;
        public TimeSpan hitTime;

        //Logic for attacking
        public bool Attacking;
        public TimeSpan previousAttackTime;
        public TimeSpan attackTime;


        // Get the width of the enemy ship
        public int Width
        {
            get { return WalkingAnimation.FrameWidth; }
        }

         //Get the height of the enemy ship
        public int Height
        {
            get { return WalkingAnimation.FrameHeight; }
        }

        // The speed at which the enemy moves
        float enemyMoveSpeed;

        public void Initialize(Animation walkingAnimation, Texture2D hitTexture, Texture2D attackingTexture, Vector2 position, bool green)
        {
            // Load the enemy ship texture
            WalkingAnimation = walkingAnimation;
            HitTexture = hitTexture;
            AttackingTexture = attackingTexture;

            // Set the position of the enemy
            Position = position;
            previousPosition = position;

            // We initialize the enemy to be active so it will be update in the game
            Active = true;
            Hit = false;
            Attacking = false;

            onGround = true;
            airborneEnemy = false;
            Green = green;

            // Set the health of the enemy green enemies have less health than red
            if (this.Green)
            {
                Health = 20;
            }
            else { Health = 40; }

            // Set the amount of damage the enemy can do
            Damage = 1;
            groundHeight = 0;
            ZAxis = 0;

            previousHitTime = TimeSpan.Zero;
            hitTime = TimeSpan.FromSeconds(0.3f);

            previousAttackTime = TimeSpan.Zero;
            attackTime = TimeSpan.FromSeconds(1.5f);

            // Set how fast the enemy moves
            enemyMoveSpeed = 1f;

            // Set the score value of the enemy
            Value = 100;
            
            //move the same direction for 25 counts in a row
            counter = 0;

        }


        public override void Update(GameTime gameTime, List<Bear> bears)     //Vector2 antonioPos, Vector2 josePos, bool antonioActive, bool joseActive)
        {
            if (!this.onGround) //didn't find a platform to walk on ie enemy walked off edge - revert to last position and move to center of screen 
            {
                this.Position = previousPosition; //revert to last spot
                this.moveRight = 0; //just move vertically
                this.counter = 0; //start new move

                //move toward center ie up or down
                if (this.Position.Y > 320)
                {
                    this.moveUp = -1;
                }
                else
                {
                    this.moveUp = 1;
                }
            }
            //restart onground process. Starts at false. If enemy is on a platform it becomes true and we're ok
            this.onGround = false;

            //Flow. cactus being punched by bear is highest priority. Then cactus attacking, then moving.
            previousPosition = Position;

            if (this.Hit)
            {
                if (gameTime.TotalGameTime - this.previousHitTime > this.hitTime) //if the cactus is hit long enough, theyre not hit anymore
                {
                    this.Hit = false;
                    if (this.Health <= 0)
                    {
                        this.Active = false;
                    }
                }
            }
            else if (this.Attacking)
            {
                if (gameTime.TotalGameTime - this.previousAttackTime > this.attackTime)
                {
                    this.Attacking = false;
                }
            }
            else //move the cactus
            {
                if (counter == 5) //only adjust cactus movement every 25 executions
                {
                    // reset the counter
                    counter = 0;

                    Vector2 bear0Pos = bears[0].Position;
                    bool bear0Active = bears[0].Active;
                    Vector2 bear1Pos;
                    bool bear1Active;

                    if (bears.Count == 2)
                    {
                        bear1Pos = bears[1].Position;
                        bear1Active = bears[1].Active;
                    }
                    else
                    {
                        bear1Active = false;
                        bear1Pos = Vector2.Zero;
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
                        WalkingAnimation.FacingRight = true;
                    }
                    else if (closerPosition.X < Position.X)
                    {
                        moveRight = -1; //move left
                        WalkingAnimation.FacingRight = false;
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

                Position.X += (enemyMoveSpeed * moveRight);
                Position.Y += (enemyMoveSpeed * moveUp);
                counter++;

                // Update the position of the Animation
                WalkingAnimation.Position = Position;

                // Update Animation
                WalkingAnimation.Update(gameTime);
            }

            //Collision Detection!!!
            Rectangle rectangle1;
            Rectangle rectangle2;

            foreach (Bear bear in bears) //THIS IS COLLISION FOR PUNCHING
            {
                //If bear isn't punching, just move to the next bear
                if (!bear.Attacking || bear.ZAxis < this.ZAxis || bear.ZAxis > this.ZAxis + this.Height / 2)
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

                rectangle2 = new Rectangle((int)this.Position.X - (this.Width / 4), (int)this.Position.Y - 25, this.Width / 2, 50);

                //Determine if overlapping 
                if (rectangle1.Intersects(rectangle2))
                {
                    this.Health -= bear.PunchDamage;
                    this.Hit = true;
                    this.previousHitTime = gameTime.TotalGameTime;

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
                            bear.previousHitTime = gameTime.TotalGameTime;
                        }
                        if (!this.Attacking) //This makes it so the cactus doesn't re-attack if another bear touches him. Bear just gets hurt
                        {
                            this.Attacking = true;
                            this.previousAttackTime = gameTime.TotalGameTime;
                        }
                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch, int xOffset)
        {

            //figure out where the bear is drawn on screen based on position and how much the level has scrolled
            Vector2 PositionOnScreen = new Vector2(Position.X - xOffset, Position.Y);

            if (Hit)//cactus hit
            {
                SpriteEffects mirror;
                if (this.WalkingAnimation.FacingRight)
                {
                    mirror = SpriteEffects.None;
                }
                else
                {
                    mirror = SpriteEffects.FlipHorizontally;
                }

                //draw the cactus being hit!
                spriteBatch.Draw(HitTexture, PositionOnScreen, null, Color.White, 0f, new Vector2(Width / 2, Height), 1f, mirror, 0f);
            }
            else if (Attacking)
            {
                //draw the cactus attacking
                spriteBatch.Draw(AttackingTexture, PositionOnScreen, null, Color.White, 0f, new Vector2(Width / 2, Height), 1f, SpriteEffects.None, 0f);
            }
            else
            {
                // Draw the animation
                WalkingAnimation.Draw(spriteBatch, PositionOnScreen);
            }


        }
    }

}
