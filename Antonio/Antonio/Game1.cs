using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace Antonio
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public struct SaveGameData
    {
        public List<string> PlayerNames;
        public List<int> PlayerKills;
    }
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Pink Bear Antonio - Player 1
        //Brown bear Jose - Player 2
        List<Bear> bears;

        Texture2D backgroundTexture;
        Vector2 backgroundPosition;
        Background desertBackground, mountainsBackground, skyBackground;
        bool scrolling;
        int xDistanceTraveled;

        //for game level/mode 0 is menu 1 is desert
        int level;
        Texture2D menuScreen;
        int menuIconPosition;
        Texture2D menuIcon;
        int players;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        // A movement speed for the player
        float playerMoveSpeed;

        // Enemies
        Texture2D GreenCactusWalking;
        Texture2D RedCactusWalking;
        Texture2D GreenCactusHit;
        Texture2D RedCactusHit;
        Texture2D GreenCactusAttacking;
        Texture2D RedCactusAttacking;
        Texture2D HawkFlying;
        Texture2D bearShadow;
        List<Enemy> enemies;
        Texture2D RawrTexture;


        //stuff for HUD
        Texture2D TacoTexture;
        Texture2D AntonioHead;
        Texture2D JoseHead;
        InstructionSign instructionSign;

        // Time variables
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        // A random number generator
        Random random;

        //Yup
        int killCounter;

        //Stuff for enemy encounters
        bool enemiesPresent;
        //EnemySpawner enemySpawner;
        int enemiesRemaining;

        //Level spawner handles level elements - boxes bridges etc.
        LevelSpawner levelSpawner;
        List<Box> currentBoxes;
        List<Taco> currentTacos;

        // The font used to display UI elements and debug text
        SpriteFont font;
        string debugText;
        string scoreBar, playerName;
        bool gameOver, takingPlayerName;
        char currentLetter; // for name entry
        SaveGameData savedata;
        StorageDevice device;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize the bears
            Bear antonio = new Bear();
            Bear jose = new Bear();
            bears = new List<Bear>();
            bears.Add(antonio);
            bears.Add(jose);

            instructionSign = new InstructionSign();

            // Set a constant player move speed
            playerMoveSpeed = 2.0f;

            //kill counter starts at zero
            killCounter = 0;
            enemiesRemaining = 0;
            enemiesPresent = false;
            //enemySpawner = new EnemySpawner();

            levelSpawner = new LevelSpawner();
            currentBoxes = new List<Box>();
            currentTacos = new List<Taco>();

            mountainsBackground = new Background();
            desertBackground = new Background();
            skyBackground = new Background();
            scrolling = false;
            xDistanceTraveled = 0;

            //menu is level 0
            level = 0;
            menuIconPosition = 0;

            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(.39f);
            
            // Initialize our random number generator
            random = new Random();

            debugText = "";
            scoreBar = "";

            savedata = new SaveGameData()
            {
                PlayerNames = new List<string>{"Don"},
                PlayerKills = new List<int>{12}
            };
            gameOver = false;
            takingPlayerName = false;
            
            playerName = "";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load the Background and put it all the way at the bottom.
            backgroundPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Height - 320);
            backgroundTexture = Content.Load<Texture2D>("BackgroundRough");
            mountainsBackground.Initialize(Content.Load<Texture2D>("MountainsBackground"), 0, GraphicsDevice.Viewport.Width, playerMoveSpeed * -.125f);
            desertBackground.Initialize(Content.Load<Texture2D>("DesertBackground"), 100, GraphicsDevice.Viewport.Width, playerMoveSpeed * -.25f);
            skyBackground.Initialize(Content.Load<Texture2D>("SkyBackground"), 0, GraphicsDevice.Viewport.Width, playerMoveSpeed * -.0625f);

            menuScreen = Content.Load<Texture2D>("Menu");
            menuIcon = Content.Load<Texture2D>("MenuIcon");

            // Load the Bears: Antonio on left side facing right, Jose is vice-versa
            // Load the player resources
            Texture2D antonioIdleTexture = Content.Load<Texture2D>("AntonioIdle");
            Texture2D antonioPunchingTexture = Content.Load<Texture2D>("AntonioPunching");
            Texture2D antonioHitTexture = Content.Load<Texture2D>("AntonioHit");
            Texture2D antonioJumping = Content.Load<Texture2D>("AntonioJumping");
            bearShadow = Content.Load<Texture2D>("CharacterShadow");
            Texture2D antonioKicking = Content.Load<Texture2D>("AntonioKicking");
            Animation antonioWalkingAnimation = new Animation();
            Texture2D antonioWalkingTexture = Content.Load<Texture2D>("AntonioWalkingAnimation");
            antonioWalkingAnimation.Initialize(antonioWalkingTexture, Vector2.Zero, 60, 100, 4, 150, Color.White, 1f, true, true);

            Vector2 antonioPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 4, GraphicsDevice.Viewport.TitleSafeArea.Y
                                                     + GraphicsDevice.Viewport.TitleSafeArea.Height * 3 / 4);

            bears[0].Initialize(antonioIdleTexture, antonioPunchingTexture, antonioHitTexture, antonioJumping, bearShadow, antonioKicking, 
                                    antonioWalkingAnimation, antonioPosition);

            Texture2D joseIdleTexture = Content.Load<Texture2D>("JoseIdle");
            Texture2D josePunchingTexture = Content.Load<Texture2D>("JosePunching");
            Texture2D joseHitTexture = Content.Load<Texture2D>("JoseHit");
            Texture2D joseJumping = Content.Load<Texture2D>("JoseJumping");
            Texture2D joseKicking = Content.Load<Texture2D>("JoseKicking");
            Animation joseWalkingAnimation = new Animation();
            Texture2D joseWalkingTexture = Content.Load<Texture2D>("JoseWalkingAnimation");
            joseWalkingAnimation.Initialize(joseWalkingTexture, Vector2.Zero, 60, 100, 4, 150, Color.White, 1f, true, true);

            Vector2 josePosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 4, GraphicsDevice.Viewport.TitleSafeArea.Y
                                                     + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            bears[1].Initialize(joseIdleTexture, josePunchingTexture, joseHitTexture, joseJumping, bearShadow, joseKicking, joseWalkingAnimation, josePosition);


            AntonioHead = Content.Load<Texture2D>("AntonioHead");
            JoseHead = Content.Load<Texture2D>("JoseHead");
            TacoTexture = Content.Load<Texture2D>("Taco");
            
            instructionSign.Initialize(Content, new Vector2(GraphicsDevice.Viewport.Width - 200, GraphicsDevice.Viewport.Height / 3));

            GreenCactusWalking = Content.Load<Texture2D>("CactusAnimation");
            GreenCactusHit = Content.Load<Texture2D>("CactusHit");
            GreenCactusAttacking = Content.Load<Texture2D>("CactusAttack");
            
            RedCactusWalking = Content.Load<Texture2D>("RedCactusAnimation");
            RedCactusHit = Content.Load<Texture2D>("RedCactusHit");
            RedCactusAttacking = Content.Load<Texture2D>("RedCactusAttack");

            HawkFlying = Content.Load<Texture2D>("HawkFlying");

            RawrTexture = Content.Load<Texture2D>("rawr");

            //enemySpawner.Initialize();

            levelSpawner.Initialize(Content);

            // Load the score font
            font = Content.Load<SpriteFont>("gameFont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) 
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            if (level > 0)  //core game loop - ie after menu
            {
                mountainsBackground.Update(scrolling);
                desertBackground.Update(scrolling);
                skyBackground.Update(scrolling);

                instructionSign.Update(gameTime);

                //Update the player
                if (!gameOver)
                {
                    UpdatePlayers(gameTime);
                }

                // Update the enemies
                UpdateEnemies(gameTime);

                //update enemy spawning based on distance travelled thru level
                if (levelSpawner.enemySpawner.Contains(xDistanceTraveled))
                {
                    enemiesPresent = true;
                    scrolling = false;
                    enemiesRemaining = (int)levelSpawner.enemySpawner[xDistanceTraveled];
                    levelSpawner.enemySpawner.Remove(xDistanceTraveled);
                    instructionSign.isVamonos = false;
                    instructionSign.isAtaque = true;
                    instructionSign.previousSignTime = gameTime.TotalGameTime;
                }

                //update level objects
                if (levelSpawner.Contains(xDistanceTraveled))
                {
                    LevelArea newArea = levelSpawner[xDistanceTraveled] as LevelArea;
                    foreach (Box box in newArea.Boxes)
                    {
                        currentBoxes.Add(box);
                    }
                    if (newArea.Tacos != null)
                    {
                        foreach (Taco taco in newArea.Tacos)
                        {
                            currentTacos.Add(taco);
                        }
                    }
                    levelSpawner.Remove(xDistanceTraveled);
                }

                for (int i = 0; i < currentBoxes.Count; i++)
                {
                    Box box = currentBoxes[i];
                    if (box.Position.X < xDistanceTraveled - box.Width)
                    {
                        currentBoxes.Remove(box);
                    }
                    box.Update(bears, enemies, gameTime);

                }
                for (int i = 0; i < currentTacos.Count; i++)
                {
                    Taco taco = currentTacos[i];
                    if (taco.Position.X < xDistanceTraveled - taco.Width)
                    {
                        currentTacos.Remove(taco);
                    }
                    taco.Update(bears);

                }
            }
            else //menu
            {
                if (currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    level = 1;
                    if (menuIconPosition == 0)
                    {
                        bears.RemoveAt(1);
                        players = 1;
                    }
                    else if (menuIconPosition == 1)
                    {
                        bears.RemoveAt(0);
                        players = 1;
                        AntonioHead = JoseHead;
                    }
                    else players = 2;
                }

                //switch btwn modes
                else if (currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
                {
                    menuIconPosition = (menuIconPosition + 1) % 3;
                }
                else if (currentKeyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
                {
                    menuIconPosition = (menuIconPosition + 2) % 3;
                }
            }

            //end of game stuff deal with save stuff
            if (!gameOver && ((!bears[0].Active && players == 1) || (players == 2 && !bears[0].Active && !bears[1].Active)))
            {
                IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                
                while (!result.IsCompleted) { }
                
                device = StorageDevice.EndShowSelector(result);
                
                gameOver = true;
                loadData();

                //player made it onto scoreboard!!! :D
                if (savedata.PlayerKills.Count < 5 || savedata.PlayerKills[4] < killCounter)
                {
                    takingPlayerName = true;
                    currentLetter = 'A';
                }
             
                
            }

            if (takingPlayerName)
            {
                takePlayerName();
            }

            base.Update(gameTime);
        }

        private void loadData()
        {

            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer("AntonioSaves", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "savegame.sav";

            // Check to see whether the save exists.
            if (!container.FileExists(filename))
            {
                // If not, dispose of the container and return.
                container.Dispose();
                return;
            }

            // Open the file.
            Stream stream = container.OpenFile(filename, FileMode.Open);

            // Convert the object to XML data and put it in the stream.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));

            savedata = (SaveGameData)serializer.Deserialize(stream);

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();

        }

        private void takePlayerName()
        {
            if (playerName.Length < 3) //don't have name yet - take mroe letters
            {
                //loop thru alphabet with up and down
                if (currentKeyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
                {
                    currentLetter = (char)((((int)currentLetter - 'A' + 1) % 26) + 'A');
                }
                if (currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
                {
                    currentLetter = (char)((((int)currentLetter - 'A' + 25) % 26) + 'A');
                }
                if (currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
                {
                    playerName += currentLetter;
                    if (playerName.Length == 3) currentLetter = ' ';
                }
            }
            else 
            {
                takingPlayerName = false;

                if (savedata.PlayerNames.Count == 5) //remove last person
                {
                    savedata.PlayerNames.RemoveAt(4);
                    savedata.PlayerKills.RemoveAt(4);
                }
                int i = 0;
                for (; i < savedata.PlayerNames.Count; i++)
                {
                    if (killCounter > savedata.PlayerKills[i])
                    {
                        break;
                    }
                }
                savedata.PlayerNames.Insert(i, playerName);
                savedata.PlayerKills.Insert(i, killCounter);

                saveData();
            }
        }
        private void saveData()
        {
            //save the data

            //StorageDevice device = StorageDevice.BeginShowSelector();

            // Open a storage container.
            IAsyncResult result = device.BeginOpenContainer("AntonioSaves", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "savegame.sav";

            // Check to see whether the save exists.
            if (container.FileExists(filename))
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);

            // Create the file.
            Stream stream = container.CreateFile(filename);

            // Convert the object to XML data and put it in the stream.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));

            serializer.Serialize(stream, savedata);

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();

        }

        private void UpdatePlayers(GameTime gameTime)
        {
            Bear bear0 = bears[0];
            if (!bear0.Hit)
            { //can't move if antonio's been hit
                bear0.Idle = true;

                // Use the Keyboard / Dpad to update antonio
                if (currentKeyboardState.IsKeyDown(Keys.F) && previousKeyboardState.IsKeyUp(Keys.F) && !bear0.inAir)
                {
                    bear0.inAir = true;
                    bear0.Idle = false;
                    bear0.velocity = 8;
                }
                else if (currentKeyboardState.IsKeyDown(Keys.D) && previousKeyboardState.IsKeyUp(Keys.D) && !bear0.Attacking)
                {
                    bear0.Attacking = true;
                    bear0.Idle = false;

                    if (!bear0.inAir)
                    {
                        bear0.previousPunchTime = gameTime.TotalGameTime;
                    }
                }
                else if (!(bear0.Attacking && !bear0.inAir)) // can move as long as you're not punching
                {
                    if (currentKeyboardState.IsKeyDown(Keys.Left) ||
                    currentGamePadState.DPad.Left == ButtonState.Pressed)
                    {
                        bear0.Position.X -= playerMoveSpeed;
                        bear0.Idle = false;

                        //make sure player faces left if you move left 
                        bear0.FacingRight = false;
                    }
                    if (currentKeyboardState.IsKeyDown(Keys.Right) ||
                    currentGamePadState.DPad.Right == ButtonState.Pressed)
                    {
                        bear0.Position.X += playerMoveSpeed;
                        bear0.Idle = false;

                        //make sure player faces right if you move right
                        bear0.FacingRight = true;

                    }
                    if (currentKeyboardState.IsKeyDown(Keys.Up) ||
                    currentGamePadState.DPad.Up == ButtonState.Pressed)
                    {
                        bear0.Position.Y -= playerMoveSpeed;
                        bear0.Idle = false;
                    }
                    if (currentKeyboardState.IsKeyDown(Keys.Down) ||
                    currentGamePadState.DPad.Down == ButtonState.Pressed)
                    {
                        bear0.Position.Y += playerMoveSpeed;
                        bear0.Idle = false;
                    }
                }
            }
            if (players == 2)
            {
                Bear bear1 = bears[1];
                // Gamepad controls player 2
                if (!bear1.Hit)
                { //can't move if jose's punching or he's been hit
                    bear1.Idle = true;

                    // Use the Keyboard / Dpad to update antonio
                    if (currentGamePadState.IsButtonDown(Buttons.A) && !previousGamePadState.IsButtonUp(Buttons.A) && !bear1.inAir)
                    {
                        bear1.inAir = true;
                        bear1.Idle = false;
                        bear1.velocity = 8;
                    }
                    else if (currentGamePadState.IsButtonDown(Buttons.X) && previousGamePadState.IsButtonUp(Buttons.X) && !bear1.Attacking)
                    {
                        bear1.Attacking = true;
                        bear1.Idle = false;
                        bear1.previousPunchTime = gameTime.TotalGameTime;
                    }
                    else if (!(bear1.Attacking && !bear1.inAir))
                    {
                        float gamePadX = currentGamePadState.ThumbSticks.Left.X / Math.Max(.5f, Math.Abs(currentGamePadState.ThumbSticks.Left.X));
                        float gamePadY = currentGamePadState.ThumbSticks.Left.Y / Math.Max(.5f, Math.Abs(currentGamePadState.ThumbSticks.Left.Y));

                        bear1.Position.X += gamePadX * playerMoveSpeed;
                        bear1.Position.Y -= gamePadY * playerMoveSpeed;
                        if ((gamePadX == 0) && (gamePadY == 0))
                        {
                            bear1.Idle = true;
                        }
                        else bear1.Idle = false;

                        //Make sure Jose is facing the right direction
                        if (gamePadX == 1)
                        {
                            bear1.FacingRight = true;
                        }
                        else if (gamePadX == -1)
                        {
                            bear1.FacingRight = false;
                        }
                    }
                }
            }

            int frameWidth = GraphicsDevice.Viewport.Width;
            int frameHeight = GraphicsDevice.Viewport.Height;
            int skyHeight = frameHeight - 280;

            

            foreach( Bear bear in bears){
                // Make sure that the player does not go out of bounds
                bear.Position.X = MathHelper.Clamp(bear.Position.X, xDistanceTraveled, xDistanceTraveled + frameWidth);
                bear.Position.Y = MathHelper.Clamp(bear.Position.Y, skyHeight, frameHeight);
            }

            

            
            //Logic for scrolling
            if (!enemiesPresent)
            {
                for (int i = 0; i < bears.Count; i++)
                {
                    if (((bears[i].Position.X - xDistanceTraveled) > ((GraphicsDevice.Viewport.Width * 3) / 4)) && !bears[i].Idle) // && !bears[i].Attacking)
                    {
                        if (players == 2) //if 2 player, make sure other player isn't preventing scrolling
                        {
                            Bear otherBear = bears[(i + 1) % 2];
                            if (otherBear.Position.X <= xDistanceTraveled && otherBear.Active)
                            {
                                //Don't scroll if the other bear is on the left
                                scrolling = false;
                                break;
                            }
                            else
                            { //other bear isn't on left side - scroll away
                                scrolling = true;
                                xDistanceTraveled += (int)playerMoveSpeed;
                                break;
                            }
                        }
                        else
                        {
                            scrolling = true;
                            xDistanceTraveled += (int)playerMoveSpeed;
                        }
                    }
                    else
                    {
                        scrolling = false;
                    }
                }
            }
            foreach (Bear bear in bears)
            {
                bear.Update(gameTime);
            }       

        }

        private void UpdateEnemies(GameTime gameTime)
        {
            
            // Spawn a new enemy enemy every 1.5 seconds
            if ((enemiesRemaining > 0) && (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime))
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                if (enemies.Count < 10)
                {
                    if (--enemiesRemaining % 5 == 0) // one in five is red
                    {
                        AddCactus(false);
                    }
                    else if (enemiesRemaining % 5 == 1)
                    {
                        AddHawk();
                    }
                    else AddCactus(true);
                    }
            }

            // Update the Enemies

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime, bears);

                if (enemies[i].Active == false)
                {
                    killCounter++;
                    enemies.RemoveAt(i);
                }
            }
            
            //This handles when players kill all enemies in a wave
            if (enemies.Count == 0 && enemiesPresent)
            {
                enemiesPresent = false;
                instructionSign.isVamonos = true;
                instructionSign.isAtaque = false;
                instructionSign.previousSignTime = gameTime.TotalGameTime;
            }
        }

        private void AddHawk()
        {
            Animation flyingAnimation = new Animation();
            Texture2D flyingTexture = HawkFlying;

            flyingAnimation.Initialize(flyingTexture, Vector2.Zero, 60, 60, 2, 200, Color.White, 1f, true, false);

            Vector2 spawnLocation = Vector2.Zero;
            bool spawnPlatformNotFound = true; 
            int counter = 0;
            while (spawnPlatformNotFound) //only try and spawn the enemy five times
            {
                //grab a random box that's currently loaded
                Box spawnPlatform = currentBoxes[random.Next(0, currentBoxes.Count)];

                if (counter > 10) //give up on spawning the enemy if you can't find a platform
                {
                    return;
                }

                //see if random box has area to the right of the screen to drop an enemy onto
                if ((spawnPlatform.Position.X + spawnPlatform.Width) > (xDistanceTraveled + GraphicsDevice.Viewport.Width + (GreenCactusHit.Width / 2)) && spawnPlatform.HowTall == 0 && spawnPlatform.Depth > 0)
                {
                    int randomY = random.Next((int) spawnPlatform.Position.Y, (int) spawnPlatform.Position.Y + spawnPlatform.Depth);
                    spawnLocation = new Vector2(xDistanceTraveled + GraphicsDevice.Viewport.Width + GreenCactusHit.Width / 2, randomY);
                    spawnPlatformNotFound = false;
                }
                //now on the left...
                else if ((spawnPlatform.Position.X < xDistanceTraveled - GreenCactusHit.Width / 2) && spawnPlatform.HowTall == 0 && spawnPlatform.Depth > 0)
                {
                    int randomY = random.Next((int) spawnPlatform.Position.Y, (int) spawnPlatform.Position.Y + spawnPlatform.Depth);
                    spawnLocation = new Vector2(xDistanceTraveled - GreenCactusHit.Width / 2, randomY);
                    spawnPlatformNotFound = false;
                }
                counter++;
            }

            // Create an enemy
            Hawk hawk = new Hawk();

            // Initialize the enemy
            hawk.Initialize(flyingAnimation, bearShadow, spawnLocation);

            // Add the enemy to the active enemies list
            enemies.Add(hawk);
        }

        private void AddCactus(bool green)
        {
            
            // Create the animation object
            Animation enemyAnimation = new Animation();
            Texture2D WalkingTexture;
            Texture2D HitTexture;
            Texture2D AttackTexture;

            if (green)
            {
                WalkingTexture = GreenCactusWalking;
                HitTexture = GreenCactusHit;
                AttackTexture = GreenCactusAttacking;
            }
            else
            {
                WalkingTexture = RedCactusWalking;
                HitTexture = RedCactusHit;
                AttackTexture = RedCactusAttacking;
            }

            // Initialize the animation with the correct animation information
           
            enemyAnimation.Initialize(WalkingTexture, Vector2.Zero, 60, 100, 2, 200, Color.White, 1f, true, false);

            Vector2 spawnLocation = Vector2.Zero;
            bool spawnPlatformNotFound = true; 
            int counter = 0;
            while (spawnPlatformNotFound) //only try and spawn the enemy five times
            {
                //grab a random box that's currently loaded
                Box spawnPlatform = currentBoxes[random.Next(0, currentBoxes.Count)];

                if (counter > 10) //give up on spawning the enemy if you can't find a platform
                {
                    return;
                }

                //see if random box has area to the right of the screen to drop an enemy onto
                if ((spawnPlatform.Position.X + spawnPlatform.Width) > (xDistanceTraveled + GraphicsDevice.Viewport.Width + (GreenCactusHit.Width / 2)) && spawnPlatform.HowTall == 0 && spawnPlatform.Depth > 0)
                {
                    int randomY = random.Next((int) spawnPlatform.Position.Y, (int) spawnPlatform.Position.Y + spawnPlatform.Depth);
                    spawnLocation = new Vector2(xDistanceTraveled + GraphicsDevice.Viewport.Width + GreenCactusHit.Width / 2, randomY);
                    spawnPlatformNotFound = false;
                }
                //now on the left...
                else if ((spawnPlatform.Position.X < xDistanceTraveled - GreenCactusHit.Width / 2) && spawnPlatform.HowTall == 0 && spawnPlatform.Depth > 0)
                {
                    int randomY = random.Next((int) spawnPlatform.Position.Y, (int) spawnPlatform.Position.Y + spawnPlatform.Depth);
                    spawnLocation = new Vector2(xDistanceTraveled - GreenCactusHit.Width / 2, randomY);
                    spawnPlatformNotFound = false;
                }
                counter++;
            }

            // Create an enemy
            Cactus cactus = new Cactus();

            // Initialize the enemy
            cactus.Initialize(enemyAnimation, HitTexture, AttackTexture, spawnLocation, green);

            // Add the enemy to the active enemies list
            enemies.Add(cactus);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightBlue);

            // Start drawing
            spriteBatch.Begin();

            if (level > 0)
            {
                //draw the background
                //spriteBatch.Draw(backgroundTexture, backgroundPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                //desert.Draw(spriteBatch);

                skyBackground.Draw(spriteBatch);
                mountainsBackground.Draw(spriteBatch);
                desertBackground.Draw(spriteBatch);


                /* debugText += "Antonio: " + antonio.Position.X + ", " + antonio.Position.Y;

                if (enemies.Count > 0)
                {
                    string cactpos = "" + enemies[0].Position.X + ", " + enemies[0].Position.Y;
                    debugText += "\nCactus: " + cactpos;
                }
                 */

                //debugText += "Kills: " + killCounter + "\n";
                //debugText += "Antonio: " + antonio.Health;
                // Draw my Debug text

               
                //reset the debug text
                debugText = "";
                scoreBar = "";


                //Make a list to hold all the non-background stuff that needs to get drawn
                //sort it by position
                List<DrawObject> stuffToDraw = new List<DrawObject>();
                foreach (Bear bear in bears)
                {
                    stuffToDraw.Add(bear);
                }
          
                // stuffToDraw.Add(taco);
                foreach (Box box in currentBoxes)
                {
                    stuffToDraw.Add(box);
                }
                foreach (Taco taco in currentTacos)
                {
                    stuffToDraw.Add(taco);
                }

                for (int i = 0; i < enemies.Count; i++)
                {
                    stuffToDraw.Add(enemies[i]);
                }

                stuffToDraw.Sort();

                for (int i = 0; i < stuffToDraw.Count; i++)
                {
                    DrawObject obj = stuffToDraw[i];
                    obj.Draw(spriteBatch, xDistanceTraveled);
                }

                Bear bear0 = bears[0];
                spriteBatch.Draw(AntonioHead, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                for (int i = 0; i < bear0.Health; i++)
                {
                    spriteBatch.Draw(TacoTexture, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + AntonioHead.Width + (i * TacoTexture.Width * .5f), GraphicsDevice.Viewport.TitleSafeArea.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }

                if (bears.Count == 2)
                {
                    spriteBatch.Draw(JoseHead, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width - JoseHead.Width, GraphicsDevice.Viewport.TitleSafeArea.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    for (int i = bears[1].Health; i > 0; i--)
                    {
                        //logic for jose's tacos (health) - have to draw left to right and end up at the edge X.X
                        Vector2 TacoVector = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width - JoseHead.Width - (.5f * TacoTexture.Width) - (i * TacoTexture.Width * .5f), GraphicsDevice.Viewport.TitleSafeArea.Y);
                        spriteBatch.Draw(TacoTexture, TacoVector, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }
                //spriteBatch.Draw(RawrTexture, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2, GraphicsDevice.Viewport.TitleSafeArea.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                instructionSign.Draw(spriteBatch, 0);


                //Draw all the text at the top and scoreboard stuff at end
                int scoreBarY = GraphicsDevice.Viewport.TitleSafeArea.Y;
                scoreBar += "Kills: " + killCounter;
                spriteBatch.DrawString(font, scoreBar, new Vector2((GraphicsDevice.Viewport.TitleSafeArea.Width / 2) - 50, scoreBarY), Color.Red);

                scoreBar = "";
                scoreBarY += 50;

                if (gameOver && takingPlayerName)
                {

                    scoreBar += ("\nGAME OVER\n\nEnter Name:" + playerName + currentLetter);
                    spriteBatch.DrawString(font, scoreBar, new Vector2((GraphicsDevice.Viewport.TitleSafeArea.Width / 2) - 50, scoreBarY), Color.Red);

                }
                else if (gameOver && !takingPlayerName)
                {
                    scoreBar += "GAME OVER\n----------------";
                    spriteBatch.DrawString(font, scoreBar, new Vector2((GraphicsDevice.Viewport.TitleSafeArea.Width / 2) - 50, scoreBarY), Color.Red);
                    scoreBar = "";
                    scoreBarY += 100;

                    for (int i = 1; i <= savedata.PlayerKills.Count; i++)
                    {
                        scoreBar += i + ". " + savedata.PlayerNames[i - 1] + " - " + savedata.PlayerKills[i - 1] + "\n";
                        spriteBatch.DrawString(font, scoreBar, new Vector2((GraphicsDevice.Viewport.TitleSafeArea.Width / 2) - 50, scoreBarY), Color.Red);
                        scoreBar = "";
                        scoreBarY += 50;
                    }
                }
                
            }
            else //draw menu
            {
                spriteBatch.Draw(menuScreen, Vector2.Zero, null, Color.White);
                spriteBatch.Draw(menuIcon, new Vector2(380, 310 + menuIconPosition * 50), null, Color.White);

            }

            // Stop drawing 
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}