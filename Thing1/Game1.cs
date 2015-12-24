#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Thing1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 

    public enum Dir
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D imgCat;
        Board board;

        int numAs;
        Dir direction;

        public Game1()
            : base()
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
            numAs = 0;
            board = new Board();
            direction = Dir.None;

            graphics.PreferredBackBufferWidth = 510;
            graphics.PreferredBackBufferHeight = 570;
            graphics.ApplyChanges();
            
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

            // TODO: use this.Content to load your game content here
            imgCat = this.Content.Load<Texture2D>("cat");
            board.LoadContent(this.Content);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            { numAs++; }

            List<int> t = new List<int>();
            //List<int>.Enumerator iter = t.GetEnumerator();
          
            if (!board.tilesMoving)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                { direction = Dir.Up; }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                { direction = Dir.Down; }
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                { direction = Dir.Left; }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                { direction = Dir.Right; }
                else
                { direction = Dir.None; }
            }
            board.Update(direction);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(35, 35, 35));
            Vector2 offset = new Vector2(50, 150);
            // TODO: Add your drawing code here

            spriteBatch.Begin();

            board.Draw(spriteBatch, offset);

            spriteBatch.End();

           

            base.Draw(gameTime);
        }
    }
}
