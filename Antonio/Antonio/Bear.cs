using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Antonio
{
    public class Bear: DrawObject
    {
        //Bear has position and height from super class

        // Textures representing the player
        public Texture2D IdleTexture;
        public Texture2D PunchingTexture;
        public Texture2D HitTexture;
        public Texture2D JumpTexture;
        public Texture2D ShadowTexture;
        public Texture2D KickingTexture;

        // Position of the Player relative to the upper left side of the screen
        //public Vector2 Position; MOVED TO SUPER CLASS

        //Whether the bear's in the air and how much
        public bool inAir;
        public float velocity; //for jumping
        public int groundHeight; //where is the shadow.

        // State of the player
        public bool Active;

        // Amount of hit points that player has
        public int Health;

        //Amount of damage punch does
        public int PunchDamage;

        //Whether bear is facing left or right
        public bool FacingRight;

        //Whether the bear is standing still
        public bool Idle;

        //Whether the bear is punching or not and 
        public bool Attacking;
        public TimeSpan previousPunchTime;
        public TimeSpan bearPunchTime;

        //Logic for bear getting hitting by an enemy
        public bool Hit;
        public bool Invincible;
        public TimeSpan previousHitTime;
        public TimeSpan bearHitTime;
        public TimeSpan bearInvinvibleTime;

        // Animation representing the player
        public Animation WalkingAnimation;

        public Vector2 previousPosition;

        // Get the width of the player ship
        public int Width
        {
            get { return WalkingAnimation.FrameWidth; }
        }

        // Get the height of the bear
        public int Height
        {
            get { return WalkingAnimation.FrameHeight; }
        }

        // Initialize the player
        public void Initialize(Texture2D idleTexture, Texture2D punchingTexture, Texture2D hitTexture, Texture2D jumpTexture, 
            Texture2D shadowTexture, Texture2D kickingTexture, Animation walkingAnimation, Vector2 position)
        {
            //Set my animations and textures
            IdleTexture = idleTexture;
            PunchingTexture = punchingTexture;
            HitTexture = hitTexture;
            WalkingAnimation = walkingAnimation;
            JumpTexture = jumpTexture;
            ShadowTexture = shadowTexture;
            KickingTexture = kickingTexture;

            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;
            previousPosition = position;

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 5;

            PunchDamage = 10;

            //player starts on ground
            ZAxis = 0;
            velocity = 0;

            previousPunchTime = TimeSpan.Zero;
            bearPunchTime = TimeSpan.FromSeconds(0.2f);
            previousHitTime = TimeSpan.Zero;
            bearHitTime = TimeSpan.FromSeconds(0.3f);
            bearInvinvibleTime = TimeSpan.FromSeconds(1.8f);

            //start bear facing right and not moving
            FacingRight = true;
            Idle = true;
            Attacking = false;
            Hit = false;
            Invincible = false;
            inAir = false;
        }

        // Update the player animation
        public void Update(GameTime gameTime)
        {

            //Make player fall if they walk off something
            if (!inAir && ZAxis > groundHeight)
            {
                inAir = true;
                velocity = 0;
            }
            
            //Bear being hit,punching, or invincible are timed states. Test to see if they're over.
            if (Hit)
            {
                if (gameTime.TotalGameTime - previousHitTime > bearHitTime)
                {
                    Hit = false;
                    if (Health <= 0)
                    {
                        this.Active = false;
                    }
                }
            }
            if (Attacking && !inAir) //punching 
            {
                if (gameTime.TotalGameTime - previousPunchTime > bearPunchTime)
                {
                    Attacking = false;
                }
            }
            if (Invincible)
            {
                if (gameTime.TotalGameTime - previousHitTime > bearInvinvibleTime)
                {
                    Invincible = false;
                }
            }
            if (inAir) //Handles falling
            {          
                velocity = Math.Max(velocity -= .3f, -9); //adjust falling speed until it hits terminal velocity
                ZAxis += velocity; //adjust where the bear is in the sky
                
                /*else if (ZAxis <= 0)
                {
                    inAir = false;
                    ZAxis = 0;
                    Attacking = false; //cancel an air-kick if you hit the ground
                }*/
                if (ZAxis < - 50 && !Hit) //bear is hit if they got below ground. They are falling into the pit
                {
                    Hit = true;
                    previousHitTime = gameTime.TotalGameTime;
                }
                else if (ZAxis < -1000)
                {
                    Health = 0;
                }

            }


            groundHeight = -1000;  //start ground height at fzfero. Find the heighest object under the player.

            WalkingAnimation.Position = Position;
            WalkingAnimation.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, int xOffset)
        {
            //update previousPosition after all is said and done
            this.previousPosition = Position;

            if (!Active)
            {
                return;
            }
            
            //figure out where the bear is drawn on screen based on position and how much the level has scrolled
            //also handles how much the bear is in the air
            Vector2 PositionOnScreen = new Vector2(Position.X - xOffset, Position.Y - ZAxis);
            
            //Figure out if sprite needs to be mirrored
            SpriteEffects mirror;
            if (this.FacingRight)
            {
                mirror = SpriteEffects.None;
            }
            else
            {
                mirror = SpriteEffects.FlipHorizontally;
            }

            //Draw the bear's shadow
            spriteBatch.Draw(ShadowTexture, new Vector2(Position.X - xOffset, Position.Y + Height - ShadowTexture.Height - groundHeight), null, Color.White, 0f, new Vector2(Width / 2, this.Height), 1f, SpriteEffects.None, 0f);

            if (inAir)
            {
                if (Attacking)
                {
                    Vector2 kickWhere;
                    if (FacingRight)
                    {
                        kickWhere = new Vector2(Width / 2, Height);
                    }
                    else
                    {
                        kickWhere = new Vector2(Width / 2 + KickingTexture.Width / 2, Height);
                    }
                    spriteBatch.Draw(KickingTexture, PositionOnScreen, null, Color.White, 0f, kickWhere, 1f, mirror, 0f);
                }
                else
                {
                    spriteBatch.Draw(JumpTexture, PositionOnScreen, null, Color.White, 0f, new Vector2(Width / 2, Height), 1f, mirror, 0f);
                }
            }
            else if (Attacking)
            {
                //draw the bear!
                spriteBatch.Draw(PunchingTexture, PositionOnScreen, null, Color.White, 0f, new Vector2(Width / 2, Height), 1f, mirror, 0f);
            }
            else if (Hit)
            {
                spriteBatch.Draw(HitTexture, PositionOnScreen, null, Color.White, 0f, new Vector2(Width / 2, Height), 1f, mirror, 0f);
            }
            else if (Idle)
            {
                //draw the bear!
                spriteBatch.Draw(IdleTexture, PositionOnScreen, null, Color.White, 0f, new Vector2(Width / 2, Height), 1f, mirror, 0f);
            }

            else
            {
                WalkingAnimation.FacingRight = this.FacingRight;
                WalkingAnimation.Draw(spriteBatch, PositionOnScreen);
            }
        }
    }
}

