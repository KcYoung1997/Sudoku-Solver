using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver_cSharp
{
    internal class Program
    {
        static Board Sudoku;
        static bool autoSolve;

        [STAThread]
        static void Main()
        {
            //Play the game infinitly, once a board has been solved restart
            do
            {
                Sudoku = new Board();
                UIinit();
                Sudoku.Init();
                TakeInBoard();
                //Ask if you want to auto-solve or watch
                ClearLines();
                Console.Write("Would you like to:\n" +
                              "- [y] Watch the board being solved\n" +
                              "- [n] Just view the result\n ");
                autoSolve = "n" == Console.ReadKey(true).KeyChar.ToString().ToLower();
                DateTime startTime = DateTime.Now;
                //Setup initial options using checkColumn/Row/Box
                SetupPossibilities();
                byte moveLevel = 0; //Store the most advanced Hidden single move we've made
                byte advancedLevel = 0;
                do
                {
                    for (int i = 0; i < 81; i++)
                    {
                        //If there is only one possibility for any square on the board make the move weve found
                        if (Sudoku.SquareOptions[i].Count(isTrue => !isTrue) == 1)
                            MakeMove(i);
                    } 
                //If there are no possible moves move to the next complexity
                moveLevel++;
                switch (moveLevel)
                {
                case 1:
                    for (byte i = 0; i < 9; i++)
                        HiddenSingle(GetRow(i));
                    break;
                case 2:
                    for (byte i = 0; i < 9; i++)
                        HiddenSingle(GetColumn(i));
                    break;
                case 3:
                    for (byte i = 0; i < 9; i++)
                        HiddenSingle(GetBox(i));
                    break;
                default:
                    moveLevel = 0;
                    advancedLevel++;
                    switch (advancedLevel)
                    {
                        case 1:
                            for (byte i = 0; i < 3; i++)
                                LockedCandidates(new[]
                                {
                                    GetRow((byte) (i*3)),
                                    GetRow((byte) (i*3 + 1)),
                                    GetRow((byte) (i*3 + 2))
                                });
                            break;
                        case 2:
                            for (int i = 0; i < 3; i++)
                                LockedCandidates(new[]
                                {
                                    GetColumn((byte) (i*3)),
                                    GetColumn((byte) (i*3 + 1)),
                                    GetColumn((byte) (i*3 + 2))
                                });
                            break;
                        case 3:
                            for (int i = 0; i < 3; i++)
                                LockedCandidates(new[]
                                {
                                    GetBox((byte) (i*3)),
                                    GetBox((byte) (i*3 + 1)),
                                    GetBox((byte) (i*3 + 2))
                                });
                            break;
                        case 4:
                            for (int i = 0; i < 3; i++)
                                LockedCandidates(new[]
                                {
                                    GetBoxRotated((byte) (i*3)),
                                    GetBoxRotated((byte) (i*3 + 1)),
                                    GetBoxRotated((byte) (i*3 + 2))
                                });
                            break;
                            //Hidden Pairs
                        case 5:
                            for (byte row = 0; row < 9; row++)
                                HiddenPlural(GetRow(row), 2);
                            break;
                        case 6:
                            for (byte column = 0; column < 9; column++)
                                HiddenPlural(GetColumn(column), 2);
                            break;
                        case 7:
                            for (byte box = 0; box < 9; box++)
                                HiddenPlural(GetBox(box), 2);
                            break;
                            //Hidden Triples
                        case 8:
                            for (byte row = 0; row < 9; row++)
                                HiddenPlural(GetRow(row), 3);
                            break;
                        case 9:
                            for (byte column = 0; column < 9; column++)
                                HiddenPlural(GetColumn(column), 3);
                            break;
                        case 10:
                            for (byte box = 0; box < 9; box++)
                                HiddenPlural(GetBox(box), 3);
                            break;
                            //Hidden quads
                        case 11:
                            for (byte row = 0; row < 9; row++)
                                HiddenPlural(GetRow(row), 4);
                            break;
                        case 12:
                            for (byte column = 0; column < 9; column++)
                                HiddenPlural(GetColumn(column), 4);
                            break;
                        case 13:
                            for (byte box = 0; box < 9; box++)
                                HiddenPlural(GetBox(box), 4);
                            break;
                            //Naked Pairs
                        case 14:
                            for (byte row = 0; row < 9; row++)
                                NakedPlural(GetRow(row), 2);
                            break;
                        case 15:
                            for (byte column = 0; column < 9; column++)
                                NakedPlural(GetColumn(column), 2);
                            break;
                        case 16:
                            for (byte box = 0; box < 9; box++)
                                NakedPlural(GetBox(box), 2);
                            break;
                            //Naked Triples
                        case 17:
                            for (byte row = 0; row < 9; row++)
                                NakedPlural(GetRow(row), 3);
                            break;
                        case 18:
                            for (byte column = 0; column < 9; column++)
                                NakedPlural(GetColumn(column), 3);
                            break;
                        case 19:
                            for (byte box = 0; box < 9; box++)
                                NakedPlural(GetBox(box), 3);
                            break;
                            //Naked quads
                        case 20:
                            for (byte row = 0; row < 9; row++)
                                NakedPlural(GetRow(row), 4);
                            break;
                        case 21:
                            for (byte column = 0; column < 9; column++)
                                NakedPlural(GetColumn(column), 4);
                            break;
                        case 22:
                            for (byte box = 0; box < 9; box++)
                                NakedPlural(GetBox(box), 4);
                            break;
                        }
                        break;
                    }
                } while (Sudoku.EmptySquares != 0 && advancedLevel < 23);
                ClearLines();
                if (Sudoku.EmptySquares == 0)
                    if (autoSolve)
                    {
                        TimeSpan finalTime = DateTime.Now - startTime;
                        Console.Write("Board solved in {0}.{1} seconds\nPress any key to start again...",
                            finalTime.Seconds, finalTime.ToString(@"fff"));
                    }
                    else
                        Console.Write("Board solved!\n" +
                                      "Press any key to start again...");
                else
                    Console.Write("Board could not be solved, sorry.\n" +
                                  "Press any key to start again...");
                Console.ReadKey(true);
                Console.Clear();
                Console.SetCursorPosition(0, 0);
            } while (true);
        }

        private static void TakeInBoard()
        {
            ClearLines();
            Console.Write("Keybindings:\n" +
                          "- [0 - 9] Enter numbers.\n" +
                          "- [Up, Down, Left, Right] Move cursor.\n" +
                          "- [Enter] Continue once done entering board.\n" +
                          "- [o] Load boards from file\n");

            Console.SetCursorPosition(17, 3);
            int cursorPos = 0;
            bool done = false;
            do
            {
                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (Console.CursorTop == 11 || Console.CursorTop == 7)
                            Console.CursorTop--;
                        if (Console.CursorTop > 3)
                        {
                            Console.CursorTop--;
                            cursorPos -= 9;
                        }
                        else
                        {
                            Console.CursorTop += 10;
                            cursorPos += 72;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (Console.CursorTop == 9 || Console.CursorTop == 5)
                            Console.CursorTop++;
                        if (Console.CursorTop < 13)
                        {
                            Console.CursorTop++;
                            cursorPos += 9;
                        }
                        else
                        {
                            Console.CursorTop -= 10;
                            cursorPos -= 72;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (Console.CursorLeft == 23 || Console.CursorLeft == 29)
                            Console.CursorLeft -= 3;
                        if (Console.CursorLeft > 17)
                        {
                            Console.CursorLeft--;
                            cursorPos--;
                        }
                        else
                        {
                            Console.CursorLeft += 14;
                            cursorPos += 8;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (Console.CursorLeft == 19 || Console.CursorLeft == 25)
                            Console.CursorLeft += 3;
                        if (Console.CursorLeft < 31)
                        {
                            Console.CursorLeft++;
                            cursorPos++;
                        }
                        else
                        {
                            Console.CursorLeft -= 14;
                            cursorPos -= 8;
                        }
                        break;
                    case ConsoleKey.Enter:
                        done = true;
                        break;
                    default:
                        switch (input.KeyChar)
                        {
                            case '0':
                                Console.Write(' ');
                                goto case '9';
                            case '1': case '2': case '3': case '4': case '5':
                            case '6': case '7': case '8': case '9':
                                Sudoku.Squares[cursorPos] = Convert.ToByte(int.Parse(input.KeyChar.ToString()));
                                if (input.KeyChar != '0')
                                    Console.Write(input.KeyChar);
                                cursorPos++;
                                if (cursorPos%3 == 0)
                                {
                                    if (cursorPos%9 == 0)
                                    {
                                        if (cursorPos%27 == 0)
                                        {
                                            if (cursorPos%81 == 0)
                                            {
                                                Console.CursorTop = 1;
                                                cursorPos = 0;
                                            }
                                            Console.CursorTop++;
                                        }
                                        Console.SetCursorPosition(17, Console.CursorTop + 1);
                                    }
                                    else Console.CursorLeft += 3;
                                }
                                break;
                            case 'O': case 'o':
                                OpenFileBoard();
                                return;
                        }
                        break;
                }
            } while (done == false);
            ClearLines();
            Console.Write("Keybindings:\n" +
                          "- [s] Save board.\n      Boards are appended to the end of the file.\n" +
                          "- [c] Continue without saving.");
            do
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case 's':
                        var openBoard = new System.Windows.Forms.OpenFileDialog();
                        openBoard.ShowDialog();
                        using (var sr = new System.IO.StreamWriter(openBoard.FileName, true))
                        {
                            //If file isn't empty, goto a new line.
                            if (sr.BaseStream.Length != 0)
                                sr.Write("\n");
                            //Write board to file
                            for (int i = 0; i < 81; i++)
                                sr.Write(Sudoku.Squares[i]);
                        }
                        return;
                    case 'c':
                        return;
                    default:
                        Console.CursorLeft--;
                        Console.Write(" ");
                        break;
                }
            } while (true);
        }

        private static void OpenFileBoard()
        {
            var openBoard = new System.Windows.Forms.OpenFileDialog
            {
                Title = "Select a text file containing board information..."
            };
            openBoard.ShowDialog();
            if (openBoard.FileName == "") TakeInBoard();
            string[] lines;
            using (var sr = new System.IO.StreamReader(openBoard.FileName))
            {
                if (sr.BaseStream.Length > 81000)
                {
                    ClearLines();
                    Console.Write(
                        "This file is large enough to have over 100 boards!Are you sure you want to open? [y to open]: ");
                    if (Console.ReadKey(true).Key.ToString().ToLower() != "y")
                    {
                        sr.Dispose();
                        TakeInBoard();
                        return;
                    }
                }
                lines = sr.ReadToEnd().Split(new[] {"\r\r\n", "\r\n", "\n"}, StringSplitOptions.None);
                if (lines.Any(ln => ln.Length != 81 ||
                    new System.Text.RegularExpressions.Regex("^[0-9]").IsMatch(ln) == false))
                {
                    ClearLines();
                    Console.WriteLine(
                        "Make sure the file you've opened:\n- Contains only numbers 0-9\n- Is 81 characters long\n\nAny key to return to entering manual board...");
                    Console.ReadKey(true);
                    sr.Dispose();
                    TakeInBoard();
                    return;
                }
            }
            ClearLines();
            Console.Write("Keybindings:\n" +
                          "- [Left, Right] Cycle through boards.\n" +
                          "- [Enter] Choose board.\n" +
                          "- [e] Edit selected board.\n" +
                          "- [q] Quit back to entering board manually.");
            byte selectedBoard = 0;
            do
            {
                for (int i = 0; i < 81; i++)
                    Display((byte) int.Parse(lines[selectedBoard][i].ToString()), i);
                string text = "Board " + (selectedBoard + 1) + "/" + (lines.Length);
                Console.SetCursorPosition(24 - (text.Length/2), 15);
                Console.Write(text);
                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (selectedBoard > 0)
                            selectedBoard--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (selectedBoard < lines.Length - 1)
                            selectedBoard++;
                        break;
                    case ConsoleKey.Enter:
                        for (int i = 0; i < 81; i++)
                            Sudoku.Squares[i] =
                                (byte) ((int) char.GetNumericValue(lines[selectedBoard][i]) & 0x000000FF);
                        Console.SetCursorPosition(0, 15);
                        Console.Write("                              ");
                        return;
                    case ConsoleKey.E:
                        for (int i = 0; i < 81; i++)
                            Sudoku.Squares[i] =
                                (byte) ((int) char.GetNumericValue(lines[selectedBoard][i]) & 0x000000FF);
                        Console.SetCursorPosition(0, 15);
                        Console.Write("                              ");
                        TakeInBoard();
                        return;
                    case ConsoleKey.Q:
                        for (int i = 0; i < 81; i++)
                            Display(0, i);
                        Console.SetCursorPosition(0, 15);
                        Console.Write("                              ");
                        TakeInBoard();
                        return;
                    default:
                        Console.CursorLeft--;
                        Console.Write(" ");
                        break;
                }
            } while (true);
        }

        private static void SetupPossibilities()
        {
            for (byte count = 0; count < 9; count++)
            {
                LoneSingle(GetRow(count));
                LoneSingle(GetColumn(count));
                LoneSingle(GetBox(count));
            }
            for (byte square = 0; square < 81; square++)
                if (Sudoku.Squares[square] != 0)
                    for (int i = 0; i < 9; i++)
                        Sudoku.SquareOptions[square][i] = true;
        }

        private static void LoneSingle(List<byte> squares)
        {
            foreach (byte square in squares)
                if (Sudoku.Squares[square] != 0) //If this square has a number
                    foreach (byte square2 in squares)
                        //Set this number as NOT an option for the rest of them
                        Sudoku.SquareOptions[square2][Sudoku.Squares[square] - 1] = true;
        }

        private static void HiddenSingle(List<byte> squares)
        {
            for (int number = 0; number < 9; number++) //For each number
            {
                
                if (squares.Count(x => Sudoku.SquareOptions[x][number] == false) != 1) continue;
                //If only one number in the list has 'number' as an option (above) then set it to a variable (below)
                byte square = squares.First(x => Sudoku.SquareOptions[x][number] == false);
                for (int option = 0; option < 9; option++)
                    if (option != number) //All options other than this number
                        Sudoku.SquareOptions[square][option] = true; //Are not options for tho
            }
        }
  
        private static void LockedCandidates(List<byte>[] sets)
        {
            //Sets could contain rows or columns but it doesn't matter to this code
            for (int third = 0; third < 3; third++) //For each subset
            {
                for (int number = 0; number < 9; number++) //For each number
                {
                    //List which sets contains the above number
                    List<int> foundIn = new[] {0, 1, 2}.Where(x => 
                        Sudoku.SquareOptions[sets[x][0 + third * 3]][number] == false ||
                        Sudoku.SquareOptions[sets[x][1 + third * 3]][number] == false ||
                        Sudoku.SquareOptions[sets[x][2 + third * 3]][number] == false).ToList();
                    if (foundIn.Count != 1) continue; //Goto next number if current number is not in only 1 third
                    for (int i = 0; i < 3; i++) // for each third
                    {
                        if (i == third) continue; //Filter current third
                        for (int square = 0; square < 3; square++) // for each square in this third
                            //remove this number as an option
                            Sudoku.SquareOptions[sets[foundIn[0]][square + i*3]][number] = true;
                    }
                }
            }
        }

        private static void HiddenPlural(List<byte> squares, byte level)
        {
            //make lists of squares which have each option
            var squaresForOption = new Dictionary<byte, List<byte>>();
            for (byte option = 0; option < 9; option++)
            {
                var optionSquares = 
                    squares.Where(square => Sudoku.SquareOptions[square][option] == false).ToList();
                //If there are 2 - level squares for this option, add it to the dictionary
                if (optionSquares.Count >= 2 && optionSquares.Count <= level)
                    squaresForOption.Add(option, optionSquares);
            }
            squaresForOption = squaresForOption.OrderByDescending(i => i.Value.Count)
                .ToDictionary(i => i.Key, i => i.Value); //Place options with 3 squares at the top
            while (squaresForOption.Count >= level && squaresForOption.First().Value.Count >= level)
            {
                KeyValuePair<byte, List<byte>> option = squaresForOption.First();
                //Remove this option from the dict so it isn't compared twice (or to itself XD)
                squaresForOption.Remove(option.Key);
                Dictionary<byte, List<byte>> options = Hidden(squaresForOption, option, level);
                if (options.Count != 0)
                {
                    options.Add(option.Key, option.Value);
                    foreach (byte square in option.Value)
                    {
                        for (byte i = 0; i < 9; i++)
                        {
                            if (options.ContainsKey(i) == false)
                            {
                                Sudoku.SquareOptions[square][i] = true;
                            }
                        }
                    }
                }
            }
        }

        private static void NakedPlural(List<byte> squares, byte level)
        {
            var optionsForSquare = new Dictionary<byte, List<byte>>();
            foreach (byte square in squares)
            {
                var optionSquares = new List<byte>();
                for (byte option = 0; option < 9; option++)
                    if (Sudoku.SquareOptions[square][option] == false)
                        optionSquares.Add(option);
                //If there are 2 - level options for this square, add it to the dictionary
                if (optionSquares.Count >= 2 && optionSquares.Count <= level)
                    optionsForSquare.Add(square, optionSquares);
            }
            //Place squares with the most options at the top
            optionsForSquare = optionsForSquare.OrderByDescending(i => i.Value.Count)
                .ToDictionary(i => i.Key, i => i.Value);
            while (optionsForSquare.Count >= level && optionsForSquare.First().Value.Count >= level)
            {
                KeyValuePair<byte, List<byte>> square = optionsForSquare.First();
                //Remove this option from the dict so it isn't compared twice (or to itself XD)
                optionsForSquare.Remove(square.Key);
                Dictionary<byte, List<byte>> squareDict = Hidden(optionsForSquare, square, level);
                if (squareDict.Count == 0) continue;
                squareDict.Add(square.Key, square.Value);
                foreach (byte sq in squares)
                {
                    if (squareDict.Keys.Contains(sq)) continue;
                    foreach (byte option in square.Value)
                        Sudoku.SquareOptions[sq][option] = true;
                }
            }
        }
        
        private static Dictionary<byte, List<byte>> Hidden(Dictionary<byte, List<byte>> squaresForOption,
            KeyValuePair<byte, List<byte>> previousOption, byte level)
        {
            level -= 1;
            while (squaresForOption.Count >= level && squaresForOption.First().Value.Count >= level)
            {
                KeyValuePair<byte, List<byte>> option = squaresForOption.First();
                //Remove this option from the dict so it isn't compared twice (or to itself XD)
                squaresForOption.Remove(option.Key);
                //If the previous option contains all our options values
                if (previousOption.Value.Intersect(option.Value).Count() != option.Value.Count) continue;
                var options = new Dictionary<byte, List<byte>>();
                //If we ain't deep enough into this rabbit hole yet
                if (level != 1)
                {
                    options = Hidden(squaresForOption, option, level);
                    if (options.Count == 0) continue; //Skip to next iteration if count==0
                    options.Add(option.Key, option.Value);
                    return options;
                }
                //If we've reached the end
                options.Add(option.Key, option.Value);
                return options;
            }
            //If no values compatible with the previousOption were found
            return new Dictionary<byte, List<byte>>();
        }

        private static void MakeMove(int square)
        {
            // Cycle through the possible moves until the only one is found,
            // edit the board to contain this number
            ClearLines();
            //Get squares column row and box number
            int column = square % 9;
            int row = (square - column) / 9;
            int box = ((column/3*3) + (row/3*27));
            int option = Sudoku.SquareOptions[square].ToList().IndexOf(false); //Option which is our move
            Console.Write("Square {0} found to be {1}", square, option + 1);
            Sudoku.Squares[square] = (byte) (option + 1);
            for (int x = 0; x < 9; x++)
                Sudoku.SquareOptions[square][x] = true;
            Display((byte) (option + 1), square);
            if (autoSolve == false)
                Console.ReadKey(true);
            //Remove move from adjacent squares
            for (int j = 0; j < 9; j++)
            {
                int[] removeFrom =
                {
                    //In adjacent row
                    row*9 + j,  
                    //In adjacent column
                    column + j*9,
                    //In adjacent box
                    box + (j/3*9) + (j%3)
                };
                for (int i = 0; i < 3; i++)
                {
                    Sudoku.SquareOptions[removeFrom[i]][option] = true;
                    //If this move limits this square to only one move, make it.
                    if (Sudoku.SquareOptions[i].Count(isTrue => !isTrue) == 1)
                        MakeMove(removeFrom[i]);
                }
            }
        }

        private static List<byte> GetRow(byte rowNumber)
        {
            return Enumerable.Range(0, 9).Select(x => (byte) (rowNumber*9 + x)).ToList();
        }

        private static List<byte> GetColumn(byte columnNumber)
        {
            return Enumerable.Range(0, 9).Select(x => (byte)(x * 9 + columnNumber)).ToList();
        }

        private static List<byte> GetBox(byte boxNumber)
        {
            return Enumerable.Range(0, 9).
                Select(x => (byte)((boxNumber / 3 * 27) + (boxNumber % 3 * 3) + (x / 3 * 9) + (x % 3))).ToList();
        }

        private static List<byte> GetBoxRotated(byte boxNumber)
        {
            return Enumerable.Range(0, 9).
                Select(x => (byte)((boxNumber / 3 * 3) + (boxNumber % 3 * 27) + (x / 3) + (x % 3 * 9))).ToList();
        }

        private static void UIinit()
        {
            Console.SetWindowSize(50, 30);
            Console.SetBufferSize(50, 30);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Title = "Sudoku Solver - Keiran Young";
            Console.WriteLine("############ Welcome to sudoku solver ############");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("               -------------------");
                for (int i2 = 0; i2 < 3; i2++)
                {
                    Console.WriteLine("               |     |     |     |");
                }
            }
            Console.WriteLine("               -------------------");
        }

        private static void ClearLines()
        {
            Console.SetCursorPosition(0, 16);
            for (int i = 0; i < 10; i++)
                Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, 16);
        }

        private static void Display(byte number, int location)
        {
            int column = location%9;
            int row = (location - column)/9;
            //Account for gaps in board
            if (row > 2)
            {
                if (row > 5)
                    row++;
                row++;
            }
            if (column > 2)
            {
                if (column > 5)
                    column += 3;
                column += 3;
            }
            Console.SetCursorPosition(17 + column, 3 + row);
            if (number == 0)
                Console.Write(" ");
            else
                Console.Write(number);
            Console.SetCursorPosition(0, 21);
        }
    }

    public class Board
    {
        //VARIABLES
        //The number in each square - 0=blank
        public byte[] Squares = new byte[81];
        //squareoptions[squarenumber] contains false for each possible number
        public bool[][] SquareOptions = new bool[81][];

        public byte EmptySquares
        {
            get { return (byte) Squares.Where(square => square == 0).Count(); }
        }

        public void Init()
        {
            for (int i = 0; i < 81; i++)
                SquareOptions[i] = new bool[9];
        }
    }
}