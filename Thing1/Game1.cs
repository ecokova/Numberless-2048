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
    /// Numberless 2048: Move tiles up, down, left, and right. Combine like tiles to 
    /// collapse them into a tile of different color.
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
        const int SCREEN_WIDTH = 510;
        const int SCREEN_HEIGHT = 570;
 
        enum GameState
        {
            Playing,
            GameOver
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D imgCat;
        Texture2D imgGameOver;
        Board board;

        KeyboardState keyboard;
        KeyboardState oldKeyboard;

        Dir direction;
        GameState gameState;
        
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
            board = new Board();
            direction = Dir.None;
            gameState = GameState.Playing;

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
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

            imgGameOver = this.Content.Load<Texture2D>("gameOverText");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            oldKeyboard = keyboard;
            keyboard = Keyboard.GetState();

            switch (gameState)
            {
                case GameState.Playing:
                    getDirection();
                    board.Update(direction);
                    if (board.isGameOver())
                    {
                        gameState = GameState.GameOver;
                    }
                    break;
                case GameState.GameOver:
                    if (wasKeyPressed(Keys.Enter))
                    {
                        this.Initialize();
                    }
                    break;
            }
            
            base.Update(gameTime);
        }

        // Determines the direction the user has chosen
        private void getDirection()
        {
            if (!board.tilesMoving)
            {
                if (wasKeyPressed(Keys.Up))
                { direction = Dir.Up; }
                else if (wasKeyPressed(Keys.Down))
                { direction = Dir.Down; }
                else if (wasKeyPressed(Keys.Left))
                { direction = Dir.Left; }
                else if (wasKeyPressed(Keys.Right))
                { direction = Dir.Right; }
                else
                { direction = Dir.None; }
            }
        }

        // Determines whether a given key was pressed, where a press occurs
        // only once per physical key depression.
        private Boolean wasKeyPressed(Keys key)
        {
            return (keyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(35, 35, 35));
            Vector2 offset = new Vector2(50, 150);
            

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.Playing:
                    board.Draw(spriteBatch, offset);
                    break;
                case GameState.GameOver:
                    board.Draw(spriteBatch, offset);
                    // NEEDSWORK: Make this more readable
                    // Draws the game over text roughly halfway above the board and as wide as the board
                    spriteBatch.Draw(imgGameOver, new Rectangle((int)offset.X, (int)offset.Y / 2,
                        SCREEN_WIDTH - (int)offset.X * 2, (int)(((float)imgGameOver.Height / imgGameOver.Width) * (SCREEN_WIDTH - offset.X * 2))), 
                        Color.White);
                    break;
            }

            spriteBatch.End();

           

            base.Draw(gameTime);
        }
    }
}
