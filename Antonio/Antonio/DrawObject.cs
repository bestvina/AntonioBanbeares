using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Antonio
{
    public abstract class DrawObject:IComparable<DrawObject>
    {
        public Vector2 Position;
       
        //this is how high off the ground something is. 
        //For level objects like the ground and boxes, it's how high its top is, ie where the player walks on it
        public float ZAxis;

        //public abstract int Height { }

        public int CompareTo(DrawObject other)
        {
            if (this.Position.Y.CompareTo(other.Position.Y) == 0)
            {
                return this.ZAxis.CompareTo(other.ZAxis);
            }
            else return this.Position.Y.CompareTo(other.Position.Y);
        }
        public abstract void Draw(SpriteBatch spritebatch, int xOffset);
    }
}
