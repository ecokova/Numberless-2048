#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Storage;
//using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Numberless_2048
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

    public class Numberless_2048 : Game
    {
        const int SCREEN_WIDTH = 510;
        const int SCREEN_HEIGHT = 570;

        enum GameState
        {
            PlayerInfo,
            Playing,
            GameOver
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D imgGameOver;
        SpriteFont defaultFont;
        Board board;

        KeyboardState keyboard;
        KeyboardState oldKeyboard;

        Dir direction;
        GameState gameState;
        string playerName;

        public Numberless_2048()
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
            gameState = GameState.PlayerInfo;

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();

            base.Initialize();

            playerName = "";           
            
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
            defaultFont = this.Content.Load<SpriteFont>("MainText");
            board.LoadContent(this.Content);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.Content.Unload();
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
                case GameState.PlayerInfo:
                    foreach (Keys key in keyboard.GetPressedKeys())
                    {
                        if (oldKeyboard.IsKeyUp(key))
                        {
                            if (key >= Keys.A && key <= Keys.Z)
                            {
                                playerName += key.ToString();
                            }
                            if (key == Keys.Back)
                            {
                                playerName = playerName.Remove(playerName.Length - 1, 1);
                            }
                            if (key == Keys.Enter)
                            {
                                gameState = GameState.Playing;
                            }
                        }
                    }
                    break;
                case GameState.Playing:
                    getDirection();
                    board.Update(direction);
                    
                    if (board.isGameOver())
                    {
                        gameState = GameState.GameOver;
                        using (StreamWriter sw = File.AppendText("scores.txt")) { 
                            DateTime date = DateTime.Now;

                            string score_string = date.ToString(new CultureInfo("en-US")) + ":\t" + playerName + "\t" + board.getScore().ToString() + "\n";
                            sw.WriteLine(score_string);
                        }
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
                case GameState.PlayerInfo:
                    offset = new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2 - 90);

                    string prompt = "Enter player name";
                    Vector2 textSize = defaultFont.MeasureString(prompt.ToString());
                    Vector2 textMidPoint = new Vector2(textSize.X / 2, textSize.Y / 2);
                    Vector2 textPosition = new Vector2((int)(textMidPoint.X - textSize.X), (int)(textMidPoint.Y - textSize.Y));
                    spriteBatch.DrawString(defaultFont, prompt.ToString(), textPosition + offset, new Color(208, 194, 183));

                    offset += new Vector2(0, textSize.Y + 20);
                    textSize = defaultFont.MeasureString(playerName.ToString());
                    textMidPoint = new Vector2(textSize.X / 2, textSize.Y / 2);
                    textPosition = new Vector2((int)(textMidPoint.X - textSize.X), (int)(textMidPoint.Y - textSize.Y));
                    spriteBatch.DrawString(defaultFont, playerName.ToString(), textPosition + offset, new Color(251, 111, 70));

                    break;
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
