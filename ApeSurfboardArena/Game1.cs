using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ApeSurfboardArena
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static public List<FighterPlayer> fighterPlayers;
        public static World world;
        List<ArenaObject> pillars;
        static public List<ArenaObject> arenaObjects;
        Random random;
        static public int windowHeight;
        static public int windowWidth;
        float width;
        int playersLeft =0;
        Animation riverAnimation;
        float height;
        enum GameState
        { menu,
            surfing,
            fighting,
            gameover
        }
        GameState gameState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            windowHeight = 1000;
            windowWidth = 1920;
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            width = ConvertUnits.ToSimUnits(windowWidth);
            height = ConvertUnits.ToSimUnits(windowHeight);
            random = new Random();
            gameState = GameState.menu;
        }

        List<ArenaObject> surfBarrels;
        protected override void Initialize()
        {
          

            base.Initialize();

            
            LoadMenu();
        }
        public void LoadFigherGame(int amount, int health, int health2)
        {
            if (world == null)
            {
                world = new World(new Vector2(0, 20));

            }
            else
            {
                world.Clear();
            }
            LoadArena();
            CreateFighterPlayers(amount, health,  health2);
        }
        string menuText;
        SpriteFont menuFont;
        public void LoadMenu()
        {

            menuFont = Content.Load<SpriteFont>("GameOverFont");
            

            menuText = "Press start to play!";
        }
        public void UpdateMenu()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)

            {
                gameState = GameState.surfing;
                LoadSurfPlayers(2);
            }
        }
      public void DrawMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(menuFont, menuText, new Vector2(windowWidth / 2, windowHeight *5/ 6), Color.Black, 0f, menuFont.MeasureString(menuText) / 2, 1f, SpriteEffects.None, 0f); ;
         
        }
        List<SurfPlayer> surfPlayers;
        Animation riverAnimationTwo;
        Animation riverAnimationThree;
        float timeSinceLastBarrel=0;
        public void AddSurfBarrel(GameTime gameTime)
        {
            timeSinceLastBarrel += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceLastBarrel >= 0.5)
            {
                Body body;
                Animation animation;


                Texture2D texture = Content.Load<Texture2D>("barrel");
                animation = new Animation(texture);
                animation.scale = 0.4f;
                body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(270 * animation.scale),
                      ConvertUnits.ToSimUnits(340 * animation.scale), 20f);


                body.BodyType = BodyType.Dynamic;
                body.IsSensor = true;
                body.CollisionCategories = Category.All;
                body.IgnoreGravity = true;
                int y = random.Next(20, windowHeight);
                body.Position = ConvertUnits.ToSimUnits(new Vector2(windowWidth + 100, y));
                body.OnCollision += barrelOnCollision ;
                ArenaObject arenaObject = new ArenaObject(animation, body);

                surfBarrels.Add(arenaObject);
                timeSinceLastBarrel = 0;
            }
           
        }

        private bool barrelOnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            foreach (SurfPlayer player in surfPlayers)
            {


                if (fixtureB.Body ==  player.body)
                {
                    
                    foreach( ArenaObject barrel in surfBarrels)
                    {
                        if(fixtureA.Body == barrel.body&& barrel.active)
                        {
                            player.hitPoints -= 10;
                            barrel.active = false;
                        }
                    }
                }
            }

            return true;
        }

        public void UpdateSurfBarrels(GameTime gameTime)
        {
            foreach(ArenaObject barrel in surfBarrels )
            {
                barrel.body.Position += ConvertUnits.ToSimUnits( new Vector2(-500, 0)* (float)gameTime.ElapsedGameTime.TotalSeconds);
                barrel.Update(gameTime);
            }
        }
        public void LoadSurfPlayers(int amount)
        {
            surfTime = 0;
              surfBarrels = new List<ArenaObject>();
            if (world == null)
            {
                world = new World(new Vector2(0, 20));

            }
            else
            {
                world.Clear();
            }

            Texture2D healthBar = Content.Load<Texture2D>("healthbar");
            Texture2D redBar = Content.Load<Texture2D>("redbar");
            Texture2D riverTex = Content.Load<Texture2D>("river");
            riverAnimation = new Animation(riverTex);
            riverTex = Content.Load<Texture2D>("river");
            riverAnimationTwo = new Animation(riverTex);

            riverTex = Content.Load<Texture2D>("river");
            riverAnimationThree = new Animation(riverTex);
            riverAnimation.Position = new Vector2(800, 540);
            riverAnimationTwo.Position = new Vector2(800*2, 540);

            riverAnimationThree.Position = new Vector2(800 * 3, 540);
            Body screenBottom = BodyFactory.CreateEdge(world, ConvertUnits.ToSimUnits(0, windowHeight),
         ConvertUnits.ToSimUnits(windowWidth, windowHeight));
            screenBottom.BodyType = BodyType.Static;
            screenBottom.CollisionCategories = Category.All;
            Body screenTop = BodyFactory.CreateEdge(world, ConvertUnits.ToSimUnits(0, 0),
                 ConvertUnits.ToSimUnits(windowWidth, 0));
            screenTop.BodyType = BodyType.Static;

            screenTop.CollisionCategories = Category.All;
            surfPlayers = new List<SurfPlayer>();
            for(int i = 0; i<amount; i++)
            {
                Texture2D texture = Content.Load<Texture2D>("surfboard");
           Animation   animation = new Animation(texture);
                animation.scale = 0.5f;
                Body body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(480 * animation.scale),
                       ConvertUnits.ToSimUnits(120* animation.scale), 10.0f);
                body.BodyType = BodyType.Dynamic;
                body.Position = ConvertUnits.ToSimUnits(400, 100*(i+2));
                body.CollisionCategories = Category.Cat2;
                body.CollidesWith = Category.Cat1;
                body.IgnoreGravity=true;
              

                SurfPlayer player = new SurfPlayer(animation, body, healthBar, redBar, (PlayerIndex)i);
                surfPlayers.Add(player);
            }
        }
        float surfTime;
        public bool IsSurfOver(GameTime gameTime)
        {
            surfTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (surfTime >45)
            {
                return true;
            }
            foreach (SurfPlayer player in surfPlayers)
            {
                if (player.hitPoints <= 50)

                {
                    return true;
                }
            }
            return false;
        }
        public void UpdateSurfPlayers(GameTime gameTime)
        {
            AddSurfBarrel(gameTime);
            UpdateSurfBarrels(gameTime);
            foreach(Player player in surfPlayers)
            {
                player.Update(gameTime);
            }
            if(IsSurfOver(gameTime))
            {
                LoadFigherGame(2, surfPlayers[0].hitPoints, surfPlayers[1].hitPoints);
                gameState = GameState.fighting;
            }
            riverAnimation.Position += new Vector2(-500, 0)* (float)gameTime.ElapsedGameTime.TotalSeconds;
            riverAnimationTwo.Position += new Vector2(-500, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            riverAnimationThree.Position += new Vector2(-500, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (riverAnimation.Position.X <= 0)
            {

                riverAnimation.Position = new Vector2(800 * 3, 540);
            }
            if (riverAnimationTwo.Position.X <= 0)
            {

                riverAnimationTwo.Position = new Vector2(800 * 3, 540);
            }
            if (riverAnimationThree.Position.X <= 0)
            {

                riverAnimationThree.Position = new Vector2(800 * 3, 540);
            }
            riverAnimation.Update(gameTime);
            riverAnimationTwo.Update(gameTime);
            riverAnimationThree.Update(gameTime);
        }
        public void DrawSurfBarrel(SpriteBatch spriteBatch)
        {
            foreach(ArenaObject barrel in surfBarrels)
            {
                barrel.Draw(spriteBatch);
            }
        }
        public void DrawSurfPlayers(SpriteBatch spriteBatch)
        {
            riverAnimation.Draw(spriteBatch);
            riverAnimationTwo.Draw(spriteBatch);

            riverAnimationThree.Draw(spriteBatch);
            DrawSurfBarrel(spriteBatch);
            foreach (Player player in surfPlayers)
            {
                player.Draw(spriteBatch);
            }
        }
        public void LoadObjects(int amount)
        {
            arenaObjects = new List<ArenaObject>();

            for (int i = 0; i < amount; i++)
            {
                Body body;
                Animation animation;
                int type = random.Next(0, 2);
                if (type ==0)
                {

                    Texture2D texture = Content.Load<Texture2D>("barrel");
                     animation = new Animation(texture);
                    animation.scale = 0.4f;
                     body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(270 * animation.scale),
                           ConvertUnits.ToSimUnits(340 * animation.scale), 20f);
                }
                else
            {
                Texture2D texture = Content.Load<Texture2D>("crate");
                     animation = new Animation(texture);
                    animation.scale = 0.25f;

                    body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(animation.frameWidth * animation.scale),
                           ConvertUnits.ToSimUnits(animation.frameHeight * animation.scale), 20f);
                }
                body.BodyType = BodyType.Dynamic;
                body.Friction = 0.8f;
                body.CollisionCategories = Category.All;
                int x = random.Next(100, windowWidth-100);
                int y = random.Next(20, windowHeight);
                body.Position = ConvertUnits.ToSimUnits( new Vector2(x,y));
                ArenaObject  arenaObject= new ArenaObject(animation, body);
                arenaObjects.Add(arenaObject);
            }
        }
        public void LoadArena()
        {
            Body screenLeft = BodyFactory.CreateEdge(world, ConvertUnits.ToSimUnits(0, 0),
                  ConvertUnits.ToSimUnits(0, windowHeight));
            screenLeft.BodyType = BodyType.Static;
            screenLeft.Friction = 0f;
            Body screenRight = BodyFactory.CreateEdge(world, ConvertUnits.ToSimUnits(windowWidth, 0),
                    ConvertUnits.ToSimUnits(windowWidth, windowHeight));
            screenRight.BodyType = BodyType.Static;
            screenRight.Friction = 0f;
            Body screenBottom = BodyFactory.CreateEdge(world, ConvertUnits.ToSimUnits(0, windowHeight),
            ConvertUnits.ToSimUnits(windowWidth, windowHeight));
            screenBottom.BodyType = BodyType.Static;
            screenBottom.CollisionCategories = Category.All;
            screenBottom.Friction = 0.8f;
            Body screenTop = BodyFactory.CreateEdge(world, ConvertUnits.ToSimUnits(0, 0),
                   ConvertUnits.ToSimUnits(windowWidth, 0));
            screenTop.BodyType = BodyType.Static;
            screenTop.Friction = 0f;
            LoadObjects(17);
            CreatePillars();
        }
        public void CreatePillars()
        {
            pillars = new List<ArenaObject>();
          
            
                Texture2D texture = Content.Load<Texture2D>("pillar");
            Animation animation = new Animation(texture);
                animation.scale =1f;

            Body body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(animation.frameWidth * animation.scale),
                       ConvertUnits.ToSimUnits(animation.frameHeight * animation.scale), 20f);
            
            body.BodyType = BodyType.Static;
            body.Friction = 0.8f;
            body.CollisionCategories = Category.All;
         
            body.Position = ConvertUnits.ToSimUnits(new Vector2(100,windowHeight- 100));

            ArenaObject arenaObject = new ArenaObject(animation, body);
            pillars.Add(arenaObject);

        

             texture = Content.Load<Texture2D>("pillar");
            animation = new Animation(texture);
            animation.scale =1f;

            body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(animation.frameWidth * animation.scale),
                   ConvertUnits.ToSimUnits(animation.frameHeight * animation.scale), 20f);

            body.BodyType = BodyType.Static;
            body.Friction = 0.8f;
            body.CollisionCategories = Category.All;

            body.Position = ConvertUnits.ToSimUnits(new Vector2(windowWidth - 100, windowHeight - 100));
             arenaObject = new ArenaObject(animation, body);
            pillars.Add(arenaObject);

        }
        public void CreateFighterPlayers(int amount,int health, int health2)
        {
            playersLeft = 0;

               fighterPlayers = new List<FighterPlayer>();
            for(int i = 0; i<amount; i++)
            {
                Texture2D healthBar = Content.Load<Texture2D>("healthbar");
                Texture2D redBar = Content.Load<Texture2D>("redbar");
                Dictionary<string, Animation> attackAnimations = new Dictionary<string, Animation>();
                Texture2D texture = Content.Load<Texture2D>("punchSwish");
                Animation animation = new Animation(texture);
           
                attackAnimations.Add("punch", animation);


                Dictionary<string, Animation> fighterAnimations = new Dictionary<string, Animation>();
                 texture = Content.Load<Texture2D>("jump");
                 animation = new Animation(texture, 5, 0.75f, new Vector2(0, 0));
                animation.runOnce = true;
                fighterAnimations.Add("jump", animation);
                 texture = Content.Load<Texture2D>("walk");
                 animation = new Animation(texture, 8, 1f, new Vector2(0, 0));
                fighterAnimations.Add("walk", animation);
                texture = Content.Load<Texture2D>("idle");
                 animation = new Animation(texture, 5,0.75f, new Vector2(0,0));
                fighterAnimations.Add("idle", animation);


                texture = Content.Load<Texture2D>("run");
                 animation = new Animation(texture, 8, 1f, new Vector2(0, 0));
                fighterAnimations.Add("run", animation);

                texture = Content.Load<Texture2D>("punch");
                animation = new Animation(texture, 5, 0.5f, new Vector2(0, 0));
                animation.runOnce = true;
                fighterAnimations.Add("punch", animation);

                texture = Content.Load<Texture2D>("death");
                animation = new Animation(texture, 5, 0.75f, new Vector2(0, 0));
                animation.runOnce = true;
                fighterAnimations.Add("death", animation);
                Body body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(120),
                       ConvertUnits.ToSimUnits(250), 10.0f);
                body.BodyType = BodyType.Dynamic;
                body.Position = ConvertUnits.ToSimUnits(100*(i*18), 100);
                body.CollisionCategories = Category.Cat4;
                body.CollidesWith = Category.Cat1 ;
                body.Friction = 0.6f;
                Body feetSensor = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(90),
                      ConvertUnits.ToSimUnits(260), 10.0f);
                feetSensor.BodyType = BodyType.Dynamic;
                feetSensor.IsSensor = true;
                feetSensor.CollisionCategories = Category.Cat3;
                feetSensor.CollidesWith = Category.Cat2;

                FighterPlayer player = new FighterPlayer(fighterAnimations, attackAnimations, healthBar, redBar, body, feetSensor,(PlayerIndex)i);
                if(i == 0)
                { player.hitPoints = health; }
                if(i==1)
                {
                    player.hitPoints = health2;
                }
                fighterPlayers.Add(player);
               



            }
        }
     

        public bool IsGameOver()
        {
            playersLeft = fighterPlayers. Count;
            foreach(FighterPlayer player in fighterPlayers)
            {
                if(player.active ==false)
                {
                    playersLeft--;
                }
            }
            if(playersLeft <= 1)
            {
                return true;
            }
            return false;
        }
        SpriteFont gameOverFont;
        String gameOverText;
        String winnerText;
        public void LoadGameOver()
        {
            gameOverFont = Content.Load<SpriteFont>("GameOverFont");
            gameOverText = "Game Over!";
            winnerText = "Player " + (winIndex + 1).ToString() + " Wins!";


        }
        public void UpdateGameOver()
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed )

            {
                gameState = GameState.menu;
                LoadMenu();
            }

        }
        public void DrawGameOver(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(gameOverFont, gameOverText, new Vector2(windowWidth / 2, windowHeight / 6), Color.Black,0f, gameOverFont.MeasureString(gameOverText) / 2, 1f, SpriteEffects.None, 0f); ;
            spriteBatch.DrawString(gameOverFont, winnerText, new Vector2(windowWidth / 2, windowHeight*2 / 6), Color.Black,0f, gameOverFont.MeasureString(winnerText) / 2,1f, SpriteEffects.None,0f);

        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            if (world == null)
            {
                world = new World(new Vector2(0, 20));

            }
            else
            {
                world.Clear();
            }

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        public void UpdateFightingGame(GameTime gameTime)
        {
            foreach (Player player in fighterPlayers)
            {
                player.Update(gameTime);
            }
            foreach (ArenaObject arenaObject in arenaObjects)
            {
                arenaObject.Update(gameTime);
            }
            foreach (ArenaObject pillar in pillars)
            {
                pillar.Update(gameTime);
            }
            if (IsGameOver())
            {
                foreach (FighterPlayer player in fighterPlayers)
                {
                    if (player.active)
                    {
                        winIndex = (int)player.playerIndex;
                    }
                }
                gameState = GameState.gameover;
                LoadGameOver();
            }
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            if(gameState ==GameState.menu)
            {
                UpdateMenu();
            }
            if(gameState ==  GameState.surfing)

            {
                UpdateSurfPlayers(gameTime);

            }
                if (gameState == GameState.fighting|| gameState == GameState.gameover )
            {
                UpdateFightingGame(gameTime);
            }
            if(gameState == GameState.gameover)
            {

                UpdateGameOver();
            }
            base.Update(gameTime);
        }
        public int winIndex=0;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();
            if (gameState == GameState.fighting || gameState == GameState.gameover)
            {

                foreach (ArenaObject pillar in pillars)
                {
                    pillar.Draw(spriteBatch);
                }
                foreach (ArenaObject arenaObject in arenaObjects)
                {
                    arenaObject.Draw(spriteBatch);
                }

                foreach (Player player in fighterPlayers)
                {
                    player.Draw(spriteBatch);
                }
                foreach (FighterPlayer player in fighterPlayers)
                {
                    player.DrawAttack(spriteBatch);
                }
            }
            if (gameState == GameState.surfing)

            {
                DrawSurfPlayers(spriteBatch);

            }
            if (gameState == GameState.gameover)
            {
                DrawGameOver(spriteBatch);
            }
            if (gameState == GameState.menu)
            {
                DrawMenu(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
