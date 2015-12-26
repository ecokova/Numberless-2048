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
        // NEEDSWORK: Should Tile be its own class? Is there enough to it to warrant that?
        const int NUM_ROWS = 4;
        const int NUM_COLS = 4;
        const int TILE_SPEED = 5;

        private Texture2D imgBoard;
        private Texture2D imgTile;
        private Random random;

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
            // NEEDSWORK: Is it confusing that there is no Initialize method? Would it be better
            // to move all this to that?
            random = new Random();
            createPositionsArray();
            createStartingBoard();
            NULLTILE.value = -1;
            tilesMoving = false;
        }

        // Creates and initializes array of positions coordinates
        private void createPositionsArray()
        {
            positions = new Vector2[NUM_ROWS, NUM_COLS];
            
            for (int row = 0; row < NUM_ROWS; row++)
            {
                for (int col = 0; col < NUM_COLS; col++)
                {
                    //                                pixel border + tile width    
                    positions[col, row] = new Vector2((2 * (col + 1)) + (100 * col), (2 * (row + 1)) + (100 * row));
                }
            }
        }

        // Creates tiles list and fills with 8 randomized tiles
        private void createStartingBoard()
        {
            tiles = new List<Tile>();

            
            //Start with 8 tiles
            while (tiles.Count() < 8)
            {
                addNewTile();
            }
        }

        // Creates a new random tile and adds it to the list of tiles
        private void addNewTile()
        {
            int pos = random.Next(0, 16);
            while (tiles.Any<Tile>(t => t.col + t.row * NUM_ROWS == pos))
            {
                pos = random.Next(0, 16);
            }

            Tile tile = new Tile();
            tile.value = random.Next(0, 2);
            tile.row = pos / NUM_ROWS;
            tile.col = pos % NUM_ROWS;
            tile.targetRow = tile.row;
            tile.targetCol = tile.col;
            tile.isMoving = false;
            tile.currentPos = positions[tile.col, tile.row];

            tiles.Add(tile);
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
                moveTiles(); 
            }
            else if (direction != Dir.None)
            {
                tilesMoving = true;
                
                for (int i = 0; i < tiles.Count; i++ )
                {
                    Tile temp = tiles[i];
                    
                    Vector2 targetCoord = getTargetCoordinate(i, direction);
                    temp.targetCol = (int)targetCoord.X;
                    temp.targetRow = (int)targetCoord.Y;
                    temp.isMoving = true;
                    tiles[i] = temp;
                }
            }
        }

        // Calculates target row and col for a tile at given index. Takes into consideration other tiles (collapse)
        private Vector2 getTargetCoordinate(int i, Dir dir)
        {
            int row = tiles[i].row;
            int col = tiles[i].col;

            List<Tile> tilesBefore;
            switch (dir)
            {
                case Dir.Up:
                    tilesBefore = tiles.FindAll(t => t.col == col && t.row <= row);
                    tilesBefore.Sort((a, b) => b.row - a.row);
                    row = 0 + countAfterCollapse(tilesBefore) - 1;
                    break;
                case Dir.Down:
                    tilesBefore = tiles.FindAll(t => t.col == col && t.row >= row);
                    tilesBefore.Sort((a, b) => a.row - b.row);
                    row = NUM_ROWS - countAfterCollapse(tilesBefore);
                    break;
                case Dir.Left:
                    tilesBefore = tiles.FindAll(t => t.row == row && t.col <= col);
                    tilesBefore.Sort((a, b) => b.col - a.col);
                    col = 0 + countAfterCollapse(tilesBefore) - 1;
                    break;
                case Dir.Right:
                    tilesBefore = tiles.FindAll(t => t.row == row && t.col >= col);
                    tilesBefore.Sort((a, b) => a.col - b.col);
                    col = NUM_COLS - countAfterCollapse(tilesBefore);
                    break;
            }
            
            return new Vector2(col, row);
        }

        // Returns the number of tiles that will remain after all collapsable tiles in a subset are combined.
        // Tiles will collapse if two tiles of the same value will be adjacent after the move.
        private int countAfterCollapse(List<Tile> subset)
        {
            // GOOGLE: Example of list.Aggregate<>
                        
            int count = subset.Count;
            for (int i = 0; i < subset.Count - 1; i++)
            {
                if (subset[i].value == subset[i + 1].value)
                {
                    count--;
                    i++;
                }
            }
            return count;
        }

        // Moves all tiles that are still moving one step towards their target coordinates
        private void moveTiles()
        {
             // Loops through tiles, for each moves the tile or marks it as done moving
            for (int i = 0; i < tiles.Count; i++) 
            {
                Tile temp = tiles[i];

                if (temp.Equals(NULLTILE))
                    continue;
                
                //Console.WriteLine("Tile " + i + ": (" + temp.row + "," + temp.col + ") => (" + temp.targetRow + "," + temp.targetCol + "), @ (" 
                //    + (int)temp.currentPos.Y / 100 + "," + (int)temp.currentPos.X / 100 + ")");
                // Tile done moving
                if (tiles[i].currentPos == positions[tiles[i].targetCol, tiles[i].targetRow])
                {
                    temp.col = temp.targetCol;
                    temp.row = temp.targetRow;
                    temp.isMoving = false;

                    // Loops through list of tiles that the current tile collapses with (should always be just one tile)
                    //NEEDSWORK: If it's just one, just use Find()/First()?
                    /*foreach (Tile e in tiles.FindAll(t => (t.currentPos == temp.currentPos) && 
                        (tiles.IndexOf(t) != i) && (temp.value == t.value))) {  
                        //NEEDSWORK: Inefficient maybe. Does IndexOf() loop through them all? O.o
                        //NEEDSWORK: Should it skip ones that aren't done moving yet?
                        
                        temp.value = (temp.value + 1) % colors.Length; 
                        tiles[tiles.IndexOf(e)] = NULLTILE;
                    }*/
                    int collapsedTile = tiles.FindIndex(t => (t.currentPos == temp.currentPos) &&
                        (tiles.IndexOf(t) != i) && (temp.value == t.value));
                    if (collapsedTile != -1)
                    {
                        temp.value = (temp.value + 1) % colors.Length;
                        tiles[collapsedTile] = NULLTILE;
                    }
                }
                else
                {
                    //NEEDSWORK: Make this faster, but not so fast that it doesn't end on the exact position
                    Vector2 step = TILE_SPEED * new Vector2(tiles[i].targetCol - tiles[i].col, tiles[i].targetRow - tiles[i].row);
                    temp.currentPos += step;
                    Vector2 pxToGo = temp.currentPos - positions[tiles[i].targetCol, tiles[i].targetRow];

                    
                    if (pxToGo.Length() < step.Length())
                    {
                        temp.currentPos = positions[tiles[i].targetCol, tiles[i].targetRow];
                        tiles[i] = temp; 
                        i--;              // Shifts iteration back to current tile; works for 0th tile too
                        continue;
                    }
                }
                tiles[i] = temp;
            }
            tilesMoving = tiles.Any<Tile>(t => t.isMoving);
            tiles.RemoveAll(t => t.Equals(NULLTILE));
            
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
