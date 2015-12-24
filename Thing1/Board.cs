using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Thing1
{
    public class Board
    {
        private Texture2D imgBoard;
        private Texture2D imgTile;

        private struct Tile
        {
            public int value;
            public int row;
            public int col;
            public int targetRow;
            public int targetCol;
            public Vector2 currentPos;
            public Boolean isMoving;
        };

        #region Array of colors
        private Color[] colors = {   
                             new Color(208,194,183), 
                             new Color(180, 157, 139), 
                             new Color(226, 82, 85),
                             new Color(245, 160, 77),
                             new Color(240, 225, 110),
                             new Color(182, 230, 120),
                             new Color(127, 206, 127),
                             new Color(108, 181, 161),
                             new Color(95, 154, 184),
                             new Color(136, 136, 196),
                             new Color(164, 124, 207),
                             new Color(191, 138, 191),
                             new Color(204, 125, 141)
                         };
        #endregion

        Tile NULLTILE;
        private Vector2[,] positions;
        private List<Tile> tiles;

        public Boolean tilesMoving;

        // Constructor / implicit "Initialize" method
        public Board()
        {
            createPositionsArray();
            createStartingBoard();
            tilesMoving = false;
        }

        // Creates and initializes array of positions coordinates
        private void createPositionsArray()
        {
            positions = new Vector2[4, 4];
            
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    //                                pixel border + tile width    
                    positions[row, col] = new Vector2((2 * (col + 1)) + (100 * col), (2 * (row + 1)) + (100 * row));
                }
            }
        }
        // Creates tiles list and fills with 8 randomized tiles
        private void createStartingBoard()
        {
            tiles = new List<Tile>();

            
            //Start with 8 tiles
            Random random = new Random();
            while (tiles.Count() < 8)
            {
                int pos = random.Next(0, 16);
                while (tiles.Any<Tile>(t => t.col + t.row * 4 == pos))
                {
                    pos = random.Next(0, 16);
                }

                Tile tile = new Tile();
                tile.value = random.Next(0, 2);
                tile.row = pos / 4;
                tile.col = pos % 4;
                tile.targetRow = tile.row;
                tile.targetCol = tile.col;
                tile.isMoving = false;
                tile.currentPos = positions[tile.row, tile.col];
                //Console.WriteLine("Tile " + tiles.Count() + ": (" + tile.row + "," + tile.col + ")"); //% DEBUG

                tiles.Add(tile);
            }
        }

        // LoadContent method
        public void LoadContent(ContentManager content)
        {
            imgBoard = content.Load<Texture2D>("board");
            imgTile = content.Load<Texture2D>("tile");
        }

        // Update method
        public void Update(Dir direction)
        {
            if (tilesMoving)
            {
                //Console.WriteLine("Tiles moving...");
                moveTiles(direction); 
            }
            else if (direction != Dir.None)
            {
                
                tilesMoving = true;
                //Console.WriteLine("Tiles not moving.");
                
                for (int i = 0; i < tiles.Count; i++ )
                {
                    Tile temp = tiles[i];
                    switch (direction)
                    {
                        case Dir.Up:
                            temp.targetRow--;
                            if (temp.targetRow < 0) temp.targetRow = 3;
                            break;
                        case Dir.Down:
                            temp.targetRow = (temp.targetRow + 1) % 4;
                            break;
                        case Dir.Left:
                            temp.targetCol--;
                            if (temp.targetCol < 0) temp.targetCol = 3;
                            break;
                        case Dir.Right:
                            temp.targetCol = (temp.targetCol + 1) % 4;
                            break;
                    }
                    if (temp.targetCol == temp.col || temp.targetRow == temp.row)
                        Console.WriteLine("*****************************************************************");
                    temp.isMoving = true;
                    tiles[i] = temp;
                }
            }
        }

        private void moveTiles(Dir _direction)
        {
            // Loops through tiles, for each moves the tile or marks it as done moving
            for (int i = 0; i < tiles.Count; i++) 
            {
                Tile temp = tiles[i];
                if (temp.Equals(NULLTILE))
                    continue;
                //Console.WriteLine("Tile " + i + ": (" + temp.row + "," + temp.col + ") => (" + temp.targetRow + "," + temp.targetCol + "), @ (" 
                //    + (int)temp.currentPos.Y / 100 + "," + (int)temp.currentPos.X / 100 + ")");
                if (tiles[i].currentPos == positions[tiles[i].targetRow, tiles[i].targetCol])
                {
                    temp.col = temp.targetCol;
                    temp.row = temp.targetRow;
                    temp.isMoving = false;

                    foreach (Tile t in tiles.FindAll(t => t.currentPos == temp.currentPos && !t.Equals(temp))) { 
                        //NEEDSWORK: Should it skip ones that aren't done moving yet?
                        //Console.WriteLine(t.currentPos.ToString() + " vs " + temp.currentPos.ToString());
                        
                        temp.value = (temp.value + 1) % colors.Length; 
                        tiles[tiles.IndexOf(t)] = NULLTILE;

                    }
                }
                else
                {
                    //Console.WriteLine("Tile " + i + " moved");
                    temp.currentPos += new Vector2((float)(tiles[i].targetRow - tiles[i].row) / 10.0f, 
                        (float)(tiles[i].targetCol - tiles[i].col) / 10.0f);
                }
                tiles[i] = temp;
            }
            //Console.WriteLine("Done with loop");
            tilesMoving = !tiles.TrueForAll(t => !t.isMoving);
            //tiles.RemoveAll(t => t.Equals(NULLTILE));
            

        }
        // Draw method
        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            //drawBoardDropShadow(spriteBatch, offset);

            spriteBatch.Draw(imgBoard, offset, Color.White);
            foreach (var t in tiles) {
                spriteBatch.Draw(imgTile, t.currentPos + offset, colors[t.value]);
            }
        }

        private void drawBoardDropShadow(SpriteBatch spriteBatch, Vector2 offset)
        {
            //spriteBatch.Draw(imgBoard., offset + new Vector2(2, 2), new Color(0, 0, 0, 32)); 

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    spriteBatch.Draw(imgBoard, 
                        new Rectangle((int)offset.X - 2 + i, (int)offset.Y - 2 + j, imgBoard.Width + 4, imgBoard.Height + 4),
                        new Color(0, 0, 0, 32));
                }
            }
                 
        }
        
    }
}
