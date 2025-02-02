namespace ConwaysGameOfLife
{
    class Program
    {
        /// <summary>
        /// Resets console colors
        /// </summary>
        static void ResetColor()
        {
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        //USER LOGIC
        /// Handles out of bounds values for Cursor's y value 
        static int HandleCursorTop(int cursorTop)
        {
            if (cursorTop < 0)
                return 0;
            if (cursorTop >= Console.BufferHeight)
                return Console.BufferHeight - 1;
            return cursorTop;
        }
        
        /// Handles out of bounds values for cursor's x value 
        static int HandleCursorLeft(int cursorLeft)
        {
            if (cursorLeft < 1)
                return 1;
            if (cursorLeft >= Console.BufferWidth)
                return Console.BufferWidth % 2 == 0 ? Console.BufferWidth - 1 : Console.BufferWidth - 2;
            return cursorLeft % 2 == 0 ? HandleCursorLeft(cursorLeft - 1) : cursorLeft;
        }
        
        /// Makes the cursor Be able to move freely until a button press
        /// If the E key is pressed, return the where the cursor is currently at
        /// If the 0 key is pressed, return the where the cursor is currently at, and indicate that user wants to leave insertion mode
        static (int left, int top, bool exit) MakeCursorMovable()
        {
            ConsoleKeyInfo k = Console.ReadKey();
            bool exit = false;

            while (k.Key != ConsoleKey.E && !exit)
            {

                switch (k.Key)
                {
                    case ConsoleKey.UpArrow:
                        Console.SetCursorPosition(HandleCursorLeft(Console.CursorLeft), HandleCursorTop(Console.CursorTop - 1));
                        break;
                    case ConsoleKey.DownArrow:
                        Console.SetCursorPosition(HandleCursorLeft(Console.CursorLeft), HandleCursorTop(Console.CursorTop + 1));
                        break;
                    case ConsoleKey.LeftArrow:
                        Console.SetCursorPosition(HandleCursorLeft(Console.CursorLeft - 2), Console.CursorTop);
                        break;
                    case ConsoleKey.RightArrow:
                        Console.SetCursorPosition(HandleCursorLeft(Console.CursorLeft + 2), Console.CursorTop);
                        break;
                    case ConsoleKey.NumPad0:
                    case ConsoleKey.D0:
                        exit = true;
                        break;
                }
                //To make sure the user doesn't need extra button presses
                if (!exit)
                    k = Console.ReadKey();

            }

            return (Console.CursorLeft, Console.CursorTop, exit);
        }
        
        /// Runs the user setup for the game
        static void GameUserSetup(bool[,] game)
        {
            (int left, int top, bool exit) = (1, 0, false);
            Console.SetCursorPosition(left, top);
            while (!exit)
            {
                PrintGame(game);
                Console.SetCursorPosition(HandleCursorLeft(left - 1), top);
                (left, top, exit) = MakeCursorMovable();
                if (!exit)
                    game[top, (left - 1) / 2] = !game[top, (left - 1) / 2];
            }
        }


        //GAME LOGIC
        /// Returns the number of neighbors for a cell
        static int GetNumOfNeighbors(bool[,] game, int x, int y)
        {
            int neighbors = 0;
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                {
                    if (i == j && i == 0) continue;
                    if (y + i < 0 || y + i >= game.GetLength(0)) continue;
                    if (x + j < 0 || x + j >= game.GetLength(0)) continue;

                    if (game[y + i, x + j])
                        neighbors++;
                }

            return neighbors;
        }


        /// responsible for updating the neighbor array
        static void UpdateNeighborArr(bool[,] game, int[,] neighborArr)
        {
            for (int y = 0; y < game.GetLength(0); y++)
                for (int x = 0; x < game.GetLength(1); x++)
                    neighborArr[y, x] = GetNumOfNeighbors(game, x, y);
        }

        /// Check to see if the cell will be alive next iteration
        static bool WillBeAlive(int[,] neighborArr, bool[,] game, int x, int y)
        {
            if (game[y, x])
                return neighborArr[y, x] == 2 || neighborArr[y, x] == 3;
            return neighborArr[y, x] == 3;
        }


        /// Updates the game with information from the updated neighbor array
        static void UpdateGame(bool[,] game, int[,] neighborArr)
        {
            UpdateNeighborArr(game, neighborArr);
            for (int y = 0; y < game.GetLength(0); y++)
                for (int x = 0; x < game.GetLength(1); x++)
                    game[y, x] = WillBeAlive(neighborArr, game, x, y);
        }

        static void Main(string[] args)
        {
            ResetColor();
            Console.Title = "Conway's Game of Life";

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("enter the game's grid size: ");
            Console.Write("X: ");
            int dimX = int.Parse(Console.ReadLine());
            Console.Write("Y: ");

            // Holds if a cell is active or inactive
            bool[,] game = new bool[int.Parse(Console.ReadLine()), dimX];
            // The neighbor array holds all neighbors for all cells
            int[,] neighborArr = new int[game.GetLength(0), game.GetLength(1)];

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("PRESS 0 TO EXIT USER MANUAL CELL INSERTION");
            Console.WriteLine("PRESS E/e INVERT A CELL'S STATE");
            ResetColor();
            Console.ReadKey();
            GameUserSetup(game);
            Console.Clear();

            Console.CursorVisible = false;
            //Game Loop
            ConsoleKey? pressedKey = null;
            while (pressedKey != ConsoleKey.X && pressedKey != ConsoleKey.Escape)
            {
                PrintGame(game);
                UpdateGame(game, neighborArr);
                pressedKey = Console.ReadKey().Key;
            }

        }

        static void PrintGame(bool[,] game)
        {
            Console.Clear();
            for (int y = 0; y < game.GetLength(0); y++)
            {
                for (int x = 0; x < game.GetLength(1); x++)
                {
                    Console.ForegroundColor = game[y, x] ? ConsoleColor.White : ConsoleColor.Black;
                    Console.Write(" #");
                }
                Console.WriteLine();

            }
            ResetColor();
        }

    }
}