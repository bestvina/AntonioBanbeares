using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Antonio
{
    class InstructionSign: DrawObject
    {
        public bool visible;
        public bool isVamonos;
        public bool isAtaque;
        public TimeSpan previousSignTime;
        public TimeSpan signFlashTime;
        Texture2D vamonosTexture;
        Texture2D ataqueTexture;

        public void Initialize(ContentManager contentManager, Vector2 position)
        {
            Position = position;
            vamonosTexture = contentManager.Load<Texture2D>("Vamonos");
            ataqueTexture = contentManager.Load<Texture2D>("Ataque");

            previousSignTime = TimeSpan.Zero;
            signFlashTime = TimeSpan.FromSeconds(4.0f);
            visible = true;
            isVamonos = true;
            isAtaque = false;
        }

        public void Update(GameTime gameTime)
        {
            if (isVamonos || isAtaque)
            {
                //make sign stop showing after flash time is over
                if (gameTime.TotalGameTime - previousSignTime > signFlashTime)
                {
                    isVamonos = false;
                    isAtaque = false;
                }
                else
                {
                    //make the sign flash every half a second
                    if (gameTime.TotalGameTime.Milliseconds % 1000 < 500)
                    {
                        visible = true;
                    }
                    else
                    {
                        visible = false;
                    }
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spritebatch, int xOffset)
        {
            if (isVamonos && visible)
            {
                spritebatch.Draw(vamonosTexture, Position, null, Color.White, 0f, new Vector2(vamonosTexture.Width / 2, vamonosTexture.Height), 1f, SpriteEffects.None, 0f);
            }
            else if (isAtaque && visible)
            {
                spritebatch.Draw(ataqueTexture, Position, null, Color.White, 0f, new Vector2(ataqueTexture.Width / 2, ataqueTexture.Height), 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
