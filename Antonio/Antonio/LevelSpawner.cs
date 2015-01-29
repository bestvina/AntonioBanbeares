using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Antonio
{
    // A hashtable. The value is a screen's worth of level. The key is when it should get added to the game ie 1 screen prior
    class LevelSpawner : Hashtable
    {
        Texture2D BoxTexture;
        Texture2D PitTexture;
        Texture2D GroundTexture;
        Texture2D TacoTexture;
        Texture2D BridgeTexture;
        Texture2D SpikeTexture;
        Texture2D TacoShack;

        public EnemySpawner enemySpawner;


        /*box1.Initialize(box2.Initialize(Content.Load<Texture2D>("Box"), new Vector2(box1.Position.X + box1.Width, box1.Position.Y), 0, true, .5f, .25f);
            box3.Initialize(Content.Load<Texture2D>("Box"), new Vector2(box2.Position.X, box2.Position.Y), box2.Height, false, .5f, .25f);
            box4.Initialize(Content.Load<Texture2D>("Box"), new Vector2(box3.Position.X + box3.Width * 2, GraphicsDevice.Viewport.Height / 2), 0, true, .5f, .25f);
            box5.Initialize(Content.Load<Texture2D>("Box"), new Vector2(box3.Position.X + box3.Width * 2, box4.Position.Y), box4.Height, false, .5f, .25f);
            ground.Initialize(Content.Load<Texture2D>("BackgroundRough"), backgroundPosition, 0, false, 1, 0);
            pit.Initialize(Content.Load<Texture2D>("EndlessPit"), new Vector2(backgroundTexture.Width, backgroundPosition.Y),  -1 * backgroundTexture.Height, false, 0, 0); 
            bridge1.Initialize(Content.Load<Texture2D>("RopeBridge"), new Vector2(pit.Position.X, GraphicsDevice.Viewport.Height / 2), 0, false, 1, 0);
            bridge2.Initialize(Content.Load<Texture2D>("RopeBridge"), new Vector2(bridge1.Position.X + bridge1.Width, GraphicsDevice.Viewport.Height / 2), 0, false, 1, 0);
            taco.Initialize(TacoTexture, new Vector2(box5.Position.X + box5.Width / 2, (int) (box5.Position.Y + box5.Height * .75)), box5.Height * 2);
        */

        public void Initialize(ContentManager Content)
        {
            BoxTexture = Content.Load<Texture2D>("Box");
            PitTexture = Content.Load<Texture2D>("EndlessPit");
            GroundTexture = Content.Load<Texture2D>("BackgroundRough");
            TacoTexture = Content.Load<Texture2D>("Taco");
            BridgeTexture = Content.Load<Texture2D>("RopeBridge");
            SpikeTexture = Content.Load<Texture2D>("Spikes");
            TacoShack = Content.Load<Texture2D>("TacoShack");
            
            List<Box> boxes = new List<Box>();
            List<Taco> tacos = new List<Taco>();
            enemySpawner = new EnemySpawner();

            //Shortcut variables
            int skyHeight = 140;
            int screenWidth = GroundTexture.Width;
            int helperX = 0;

            //first screen - nothing
            int distance = 0;
            boxes.Add(new Box(GroundTexture, new Vector2(distance, skyHeight), 0, false, 1, 0, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + 600, 300), 0, false, .5f, .25f, false));
            tacos.Add(new Taco(TacoTexture, new Vector2(distance + 600 + (BoxTexture.Width / 2), 300 + (BoxTexture.Height * 3 / 8)), BoxTexture.Height / 2));
            
            enemySpawner.Add(screenWidth / 4, 5);

            //second screen - line of boxes. Learn to jump
            distance += screenWidth;
            boxes.Add(new Box(GroundTexture, new Vector2(distance, skyHeight), 0, false, 1, 0, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth / 2, skyHeight), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth / 2, skyHeight + (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth / 2, skyHeight + 2 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth / 2, skyHeight + 3 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth / 2, skyHeight + 4 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));

            enemySpawner.Add(screenWidth, 10);

            //spawn first two screens together
            this.Add(0, new LevelArea(boxes, tacos));

            
            
            /*this is the pool
            boxes.Add(new Box(GroundTexture, new Vector2(distance, skyHeight), 0, false, 1, 0, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + 250, 350), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + 250 + BoxTexture.Width, 350),0,true,.5f,.25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + 250 + BoxTexture.Width, 350),BoxTexture.Height / 2,false,.5f,.25f, false));
            boxes.Add(new Box(TacoShack, new Vector2(distance + 250 + 3 * BoxTexture.Width, 320), 0, false, .33f, .1f, false));
            tacos.Add(new Taco(TacoTexture, new Vector2(distance + 250 + (3 * BoxTexture.Width) + (TacoShack.Width / 2), 320 + TacoShack.Height / 4), (TacoShack.Height * 2) / 3));
            //boxes.Add(new Box(BoxTexture, new Vector2(250 + 3 * BoxTexture.Width, 350), BoxTexture.Height / 2, false, .5f, .25f, false));
            boxes.Add(new Box(BridgeTexture, new Vector2(distance + 750, skyHeight + 100), 0, false, 1, .1f, false));
            //boxes.Add(new Box(SpikeTexture, new Vector2(distance, skyHeight + 20), 0, false, 1, 0, true)); 
            */
           
            //third screen
            distance += screenWidth;
            boxes = new List<Box>();
            tacos = new List<Taco>();
            boxes.Add(new Box(GroundTexture, new Vector2(distance, skyHeight), 0, false, 1, 0, false));
            helperX = distance + screenWidth / 6;
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 2 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 3 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 4 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(SpikeTexture, new Vector2(helperX + BoxTexture.Width, skyHeight + 20), 0, false, 1, 0, true));
            boxes.Add(new Box(SpikeTexture, new Vector2(helperX + BoxTexture.Width + SpikeTexture.Width, skyHeight + 20), 0, false, 1, 0, true));
            helperX = helperX + BoxTexture.Width + SpikeTexture.Width + SpikeTexture.Width;
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 2 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 3 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 4 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(SpikeTexture, new Vector2(helperX + BoxTexture.Width, skyHeight + 20), 0, false, 1, 0, true));
            boxes.Add(new Box(SpikeTexture, new Vector2(helperX + BoxTexture.Width + SpikeTexture.Width, skyHeight + 20), 0, false, 1, 0, true));
            helperX = helperX + BoxTexture.Width + SpikeTexture.Width + SpikeTexture.Width;
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 2 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 3 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 4 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(SpikeTexture, new Vector2(helperX + BoxTexture.Width, skyHeight + 20), 0, false, 1, 0, true));
            boxes.Add(new Box(SpikeTexture, new Vector2(helperX + BoxTexture.Width + SpikeTexture.Width, skyHeight + 20), 0, false, 1, 0, true));
            helperX = helperX + BoxTexture.Width + SpikeTexture.Width + SpikeTexture.Width;
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 2 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 3 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(helperX, skyHeight + 4 * (BoxTexture.Height / 2)), 0, false, .5f, .25f, false));
            //boxes.Add(new Box(SpikeTexture, new Vector2(helperX + BoxTexture.Width, skyHeight + 20), 0, false, 1, 0, true));
            //boxes.Add(new Box(SpikeTexture, new Vector2(helperX + BoxTexture.Width + SpikeTexture.Width, skyHeight + 20), 0, false, 1, 0, true));
            helperX = helperX + BoxTexture.Width + SpikeTexture.Width + SpikeTexture.Width;
            
            this.Add(screenWidth, new LevelArea(boxes, null));

            //4th
            distance += screenWidth;
            boxes = new List<Box>();
            tacos = new List<Taco>();
            boxes.Add(new Box(GroundTexture, new Vector2(distance, skyHeight), 0, false, 1, 0, false));
            boxes.Add(new Box(TacoShack, new Vector2((int)(distance + (screenWidth / 2) - (TacoShack.Width / 2)), skyHeight + 1), 0, false, .33f, .1f, false));
            boxes.Add(new Box(BridgeTexture, new Vector2(distance + 750, skyHeight + 100), 0, false, 1, .1f, false));
            this.Add(screenWidth * 2, new LevelArea(boxes, null));

            enemySpawner.Add(distance, 15);
            
            
            /*
            boxes.Add(new Box(PitTexture, new Vector2(distance, skyHeight), -1 * PitTexture.Height, false, 0, 0, false));
            boxes.Add(new Box(BridgeTexture, new Vector2(distance + BridgeTexture.Width - 50, skyHeight + 100), 0, false, 1, .1f, false));
            boxes.Add(new Box(BridgeTexture, new Vector2(distance + 2 * BridgeTexture.Width - 50, skyHeight + 100), 0, false, 1, .1f, false));
            boxes.Add(new Box(BridgeTexture, new Vector2(distance + 3 * BridgeTexture.Width - 50, skyHeight + 100), 0, false, 1, .1f, false));
            boxes.Add(new Box(BridgeTexture, new Vector2(distance + 4 * BridgeTexture.Width - 50, skyHeight + 100), 0, false, 1, .1f, false));
            this.Add(screenWidth * 2, new LevelArea(boxes, null));*/


            //5th
            distance += screenWidth;
            boxes = new List<Box>();
            boxes.Add(new Box(PitTexture, new Vector2(distance, skyHeight), -1 * PitTexture.Height, false, 0, 0, false));
            boxes.Add(new Box(BridgeTexture, new Vector2(distance + BridgeTexture.Width - 50, skyHeight + 100), 0, false, 1, .1f, false));
            boxes.Add(new Box(BridgeTexture, new Vector2(distance + 2 * BridgeTexture.Width - 50, skyHeight + 100), 0, false, 1, .1f, false));
            boxes.Add(new Box(BridgeTexture, new Vector2(distance + 3 * BridgeTexture.Width - 50, skyHeight + 100), 0, false, 1, .1f, false));
            boxes.Add(new Box(BridgeTexture, new Vector2(distance + 4 * BridgeTexture.Width - 50, skyHeight + 100), 0, false, 1, .1f, false));
            

            this.Add(screenWidth * 3, new LevelArea(boxes, null));

            enemySpawner.Add(distance, 20);

            //6th screen - the end - 200 enemies and a wall
            distance = distance + screenWidth;

            boxes = new List<Box>();
            tacos = new List<Taco>();
            tacos.Add(new Taco(TacoTexture, new Vector2(distance + 300, 250), 0));
            tacos.Add(new Taco(TacoTexture, new Vector2(distance + 300, 350), 0));

            boxes.Add(new Box(GroundTexture, new Vector2(distance, skyHeight), 0, false, 1, 0, false));
            boxes.Add(new Box(GroundTexture, new Vector2(distance + screenWidth, skyHeight), 0, false, 1, 0, false));
   
            //final ring
            boxes.Add(new Box(SpikeTexture, new Vector2(distance + 100, skyHeight + 20), 0, false, 1, 0, true));
            boxes.Add(new Box(SpikeTexture, new Vector2(distance + 100 + screenWidth - SpikeTexture.Width, skyHeight + 20), 0, false, 1, 0, true));

            //end of game! Boxes and spikes
            distance = distance + screenWidth - BoxTexture.Width;
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight), 0, true, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + (BoxTexture.Height / 2)), 0, true, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 2 * (BoxTexture.Height / 2)), 0, true, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 3 * (BoxTexture.Height / 2)), 0, true, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 4 * (BoxTexture.Height / 2)), 0, true, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight), BoxTexture.Height / 2, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + (BoxTexture.Height / 2)), BoxTexture.Height / 2, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 2 * (BoxTexture.Height / 2)), BoxTexture.Height / 2, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 3 * (BoxTexture.Height / 2)), BoxTexture.Height / 2, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 4 * (BoxTexture.Height / 2)), BoxTexture.Height / 2, false, .5f, .25f, false));

            


            distance = distance + BoxTexture.Width;
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight), 0, true, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + (BoxTexture.Height / 2)), 0, true, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 2 * (BoxTexture.Height / 2)), 0, true, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 3 * (BoxTexture.Height / 2)), 0, true, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 4 * (BoxTexture.Height / 2)), 0, true, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight), BoxTexture.Height / 2, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + (BoxTexture.Height / 2)), BoxTexture.Height / 2, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 2 * (BoxTexture.Height / 2)), BoxTexture.Height / 2, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 3 * (BoxTexture.Height / 2)), BoxTexture.Height / 2, false, .5f, .25f, false));
            boxes.Add(new Box(BoxTexture, new Vector2(distance + screenWidth - BoxTexture.Width, skyHeight + 4 * (BoxTexture.Height / 2)), BoxTexture.Height / 2, false, .5f, .25f, false));
            boxes.Add(new Box(SpikeTexture, new Vector2(distance + screenWidth - (BoxTexture.Width * 2) - SpikeTexture.Width, skyHeight + 20), 0, false, 1, 0, true));

            enemySpawner.Add(distance - screenWidth + 100, 200);

            this.Add(screenWidth * 4, new LevelArea(boxes, tacos));
            
            
        }

    }
}
