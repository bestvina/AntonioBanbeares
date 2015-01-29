using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Antonio
{
    public abstract class Enemy: DrawObject
    {
        public bool Active;

        //numbers to determine which directions enemy should walk
        public int counter, moveRight, moveUp;

        public Vector2 previousPosition;

        //logic for whether the enemy is airborne and whether they have ground on their feet. Shouldn't walk into pit
        public bool airborneEnemy;
        public bool onGround;
        public int groundHeight; //where's my shadow?

        public abstract void Update(GameTime gametime, List<Bear> bears); 
    }
}
