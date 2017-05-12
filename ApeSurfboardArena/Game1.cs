using FarseerPhysics;
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
        List<FighterPlayer> fighterPlayers;
        public static World world;
        
        static public int windowHeight;
        static public int windowWidth;
        float width;

        float height;
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
       
        }

       
        protected override void Initialize()
        {
          

            base.Initialize();
            LoadArena();
            CreateFighterPlayers(2);
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
            screenBottom.Friction = 0.6f;
            Body screenTop = BodyFactory.CreateEdge(world, ConvertUnits.ToSimUnits(0, 0),
                   ConvertUnits.ToSimUnits(windowWidth, 0));
            screenTop.BodyType = BodyType.Static;
            screenTop.Friction = 0f;

        }

        public void CreateFighterPlayers(int amount)
        {
            
            fighterPlayers = new List<FighterPlayer>();
            for(int i = 0; i<amount; i++)
            {
                Dictionary<string, Animation> fighterAnimations = new Dictionary<string, Animation>();
                Texture2D texture = Content.Load<Texture2D>("Ape");
                Animation animation = new Animation(texture);
                fighterAnimations.Add("idle", animation);
                Body body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(150),
                       ConvertUnits.ToSimUnits(250), 10.0f);
                body.BodyType = BodyType.Dynamic;
                body.Position = ConvertUnits.ToSimUnits(100, 100);
                body.CollisionCategories = Category.Cat4;
                body.CollidesWith = Category.Cat1 ;
                body.Friction = 0.6f;
                Body feetSensor = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(150),
                      ConvertUnits.ToSimUnits(251), 10.0f);
                feetSensor.BodyType = BodyType.Dynamic;
                feetSensor.IsSensor = true;
                feetSensor.CollisionCategories = Category.Cat3;
                feetSensor.CollidesWith = Category.Cat2;

                FighterPlayer player = new FighterPlayer(fighterAnimations, body, feetSensor,(PlayerIndex)i);
                fighterPlayers.Add(player);




            }
        }
        public void LoadFighterAnimations()
        {
           

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
            LoadFighterAnimations();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            foreach (Player player in fighterPlayers)
            {
                player.Update(gameTime);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();

            
          foreach(Player player in fighterPlayers)
            {
                player.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
