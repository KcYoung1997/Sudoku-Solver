using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver_cSharp
{
    class Program
    {
        static bool autoSolve = false;
        static byte moveLevel = 0;
        static byte lockedLevel = 0;
        [STAThread]
        static void Main(string[] args)
        {
            //Play the game infinitly, once a board has been solved restart
            do
            {
                UIinit();
                Board Sudoku = new Board();
                Sudoku.init();
                TakeInBoard(Sudoku.squares);
                //Ask if you want to auto-solve or watch
                ClearLines();
                Console.Write("Would you like to:\n" +
                    "- [y] Watch the board being solved\n" +
                    "- [n] Just view the result\n ");
                if (Console.ReadKey(true).KeyChar.ToString().ToLower() == "n")
                { autoSolve = true; }
                else
                { autoSolve = false; }
                DateTime startTime = DateTime.Now;
                //Setup initial options using checkColumn/Row/Box
                SetupPossibilities(Sudoku);
                byte moveLevel = 0; //Store the most advanced Hidden row move we've made
                byte lockedLevel = 0; //store the most advanced locked candidate move we've checked
                do
                {
                    for (int i = 0; i < 81; i++)
                    {
                        //Count possible moves for this square
                        var possibleMoves =
                            from x in Sudoku.squareOptions[i]
                            where x == false
                            select x;
                        //If there is only one possibility for any square on the board
                        //revert complexity back to basic moves and make the move weve found
                        if (possibleMoves.Count() == 1)
                        {
                            MakeMove(Sudoku, i, moveLevel);
                        }
                    }
                    //If there are no possible moves move to the next complexity
                    moveLevel++;
                    if (moveLevel < 4)
                    {
                        if (moveLevel == 1)
                        {
                            for (int i = 0; i < 9; i++) { HiddenRow(Sudoku, i); }
                        }
                        if (moveLevel == 2)
                        {
                            for (int i = 0; i < 9; i++) { HiddenColumn(Sudoku, i); }
                        }
                        if (moveLevel == 3)
                        {
                            for (int i = 0; i < 9; i++) { HiddenBox(Sudoku, i); }
                        }
                    }
                    else
                    {
                        //start going through locked candidates again after this
                        moveLevel = 0;
                        lockedLevel++;
                        if (lockedLevel == 1)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                //lockedBoxRow(Sudoku, i);
                            }
                        }
                        if (lockedLevel == 2)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                //lockedBoxColumn(Sudoku, i);
                            }
                        }
                    }
                } while (Sudoku.EmptySquares() != 0 && lockedLevel != 3);
                ClearLines();
                if (Sudoku.EmptySquares() == 0)
                {
                    if (autoSolve)
                    {
                        TimeSpan finalTime = DateTime.Now - startTime;
                        Console.Write("Board solved in {0}.{1} seconds\n" +
                            "Press any key to start again...", finalTime.Seconds.ToString(), finalTime.Milliseconds.ToString());
                    }
                    else
                    {
                        Console.Write("Board solved!\n" +
                            "Press any key to start again...");
                    }
                }
                else
                {
                    Console.Write("Board could not be solved, sorry.\n" +
                                "Press any key to start again...");
                }
                Console.ReadKey(true);
                Console.Clear();
                Console.SetCursorPosition(0, 0);
            } while (true);
        }

        static void TakeInBoard(byte[] squares)
        {
            ClearLines();
            Console.Write("Keybindings:\n"+
                "- [0 - 9] Enter numbers.\n"+
                "- [Up, Down, Left, Right] Move cursor.\n"+
                "- [Enter] Continue once done entering board.\n"+
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
                        { 
                            Console.CursorTop--;
                        }
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
                        {
                            Console.CursorTop++; 
                        }
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
                        {
                            Console.CursorLeft -= 3;
                        }
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
                        {
                            Console.CursorLeft += 3;
                        }
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
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                                squares[cursorPos] = Convert.ToByte(int.Parse(input.KeyChar.ToString()));
                                if (input.KeyChar != '0')
                                {
                                    Console.Write(input.KeyChar);
                                }
                                cursorPos++;
                                if (cursorPos % 3 == 0)
                                {
                                    if (cursorPos % 9 == 0)
                                    {
                                        if (cursorPos % 27 == 0)
                                        {
                                            if (cursorPos % 81 == 0)
                                            {
                                                Console.CursorTop = 1;
                                                cursorPos = 0;
                                            }
                                            Console.CursorTop++;
                                        }
                                        Console.SetCursorPosition(17, Console.CursorTop + 1);
                                    }
                                    else
                                    {
                                        Console.CursorLeft += 3;
                                    };
                                }

                                break;
                            case 'O':
                            case 'o':
                                OpenFileBoard(squares);
                                return;
                            default:
                                break;
                        }
                        break;
                }
            } while (done == false);
            ClearLines();
            Console.Write("Keybindings:\n" +
                "- [s] Save board.\n      Boards are appended to the end of the file.\n" +
                "- [c] Continue without saving.");
            done = false;
            do
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case 's':
                        System.Windows.Forms.OpenFileDialog openBoard = new System.Windows.Forms.OpenFileDialog();
                        openBoard.ShowDialog();
                        using(System.IO.StreamWriter sr = new System.IO.StreamWriter(openBoard.FileName, true))
                        {
                            sr.Write("\n");
                            for (int i = 0; i < 81; i++)
                            {
                                sr.Write(squares[i]);
                            }
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
        static void OpenFileBoard(byte[] squares)
        {
            System.Windows.Forms.OpenFileDialog openBoard = new System.Windows.Forms.OpenFileDialog();
            openBoard.Title = "Select a text file containing board information...";
            openBoard.ShowDialog();
            string[] lines;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(openBoard.FileName))
            {
                if (sr.BaseStream.Length > 81000)
                {
                    ClearLines();
                    Console.Write("This file is large enough to have over 100 boards!Are you sure you want to open? [y to open]: ");
                    if (Console.ReadKey(true).Key.ToString().ToLower() != "y")
                    {
                        sr.Dispose();
                        TakeInBoard(squares);
                        return;
                    }

                }
                lines = sr.ReadToEnd().Split(new string[] { "\r\r\n", "\r\n", "\n" }, StringSplitOptions.None);
                foreach (string ln in lines)
                {
                    if (ln.Length != 81 || new System.Text.RegularExpressions.Regex("^[0-9]").IsMatch(ln) == false)
                    {
                        ClearLines();
                        Console.WriteLine("Make sure the file you've opened:\n- Contains only numbers 0-9\n- Is 81 characters long\n\nAny key to return to entering manual board...");
                        Console.ReadKey(true);
                        sr.Dispose();
                        TakeInBoard(squares);
                        return;
                    }
                }
            }
            ClearLines();
            Console.Write("Keybindings:\n" +
                "- [Left, Right] Cycle through boards.\n"+
                "- [Enter] Choose board.\n"+
                "- [e] Edit selected board.\n"+
                "- [q] Quit back to entering board manually.");
            byte selectedBoard = 0;
            bool done = false;

            do
            {
                for (int i = 0; i < 81; i++)
                {
                    Display((byte)int.Parse(lines[selectedBoard][i].ToString()), i);
                }
                string text = "Board " + (selectedBoard+1) + "/" + (lines.Length);
                Console.SetCursorPosition(24 - (text.Length / 2), 15);
                Console.Write(text);
                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (selectedBoard > 0)
                        {
                            selectedBoard--;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (selectedBoard < lines.Length - 1)
                        {
                            selectedBoard++;
                        }
                        break;
                    case ConsoleKey.Enter:
                        for (int i = 0; i < 81; i++)
                        {
                            squares[i] = (byte)((int)char.GetNumericValue(lines[selectedBoard][i]) & 0x000000FF);
                        }
                        Console.SetCursorPosition(0, 15);
                        Console.Write("                              ");
                        return;
                    case ConsoleKey.E:
                        for (int i = 0; i < 81; i++)
                        {
                            squares[i] = (byte)((int)char.GetNumericValue(lines[selectedBoard][i]) & 0x000000FF);
                        }
                        Console.SetCursorPosition(0, 15);
                        Console.Write("                              ");
                        TakeInBoard(squares);
                        return;
                    case ConsoleKey.Q:
                        for (int i = 0; i < 81; i++)
                        {
                            Display(0, i);
                        }
                        Console.SetCursorPosition(0, 15);
                        Console.Write("                              ");
                        TakeInBoard(squares);
                        return;
                    default:
                        Console.CursorLeft--;
                        Console.Write(" ");
                        break;
                }
            } while (done == false);
        }

        static void SetupPossibilities(Board Sudoku)    
        {
            for (byte count = 0; count < 9; count++)
            {
                LoneRow(Sudoku, count);
                LoneColumn(Sudoku, count);
                LoneBox(Sudoku, count);
            }
            for (byte square = 0; square < 81; square++)
            {
                if (Sudoku.squares[square] != 0)
                {

                    for (int i = 0; i < 9; i++)
	                {
                        Sudoku.squareOptions[square][i] = true;
	                }
                }

            }
        }
        static void LoneRow(Board Sudoku, byte row)
        {
            for (int searchSquare = 0; searchSquare < 9; searchSquare++) //For each square in this row
            {
                if (Sudoku.squares[row * 9 + searchSquare] != 0) //If this square has a number
                {
                    for (int square = 0; square < 9; square++)
                    {
                        //Set this number as NOT an option for the rest of the row
                        Sudoku.squareOptions[row * 9 + square][Sudoku.squares[row * 9 + searchSquare] - 1] = true;
                    }
                }
            }
        }
        static void LoneColumn(Board Sudoku, byte column)
        {
            for (int searchSquare = 0; searchSquare < 9; searchSquare++) //For each square in this column
            {
                if (Sudoku.squares[column + searchSquare * 9] != 0) //If this square has a number
                {
                    for (int square = 0; square < 9; square++)
                    {
                        //Set this number as NOT an option for the rest of the column
                        Sudoku.squareOptions[column + square * 9][Sudoku.squares[column + searchSquare * 9] - 1] = true;
                    }
                }
            }
        }
        static void LoneBox(Board Sudoku, byte box)
        {
            int startposition = ((box / 3) * 27) + ((box % 3) * 3);
            for (int searchSquare = 0; searchSquare < 9; searchSquare++) //For each square in this box
            {
                if (Sudoku.squares[startposition + ((searchSquare % 3) + (searchSquare / 3 * 9))] != 0) //If this square has a number
                {
                    for (int square = 0; square < 9; square++)
                    {
                        //Set this number as NOT an option for the rest of the box
                        Sudoku.squareOptions[startposition + ((square % 3) + (square / 3 * 9))][Sudoku.squares[startposition + ((searchSquare % 3) + (searchSquare / 3 * 9))] - 1] = true;
                    }
                }
            }
        }

        static void HiddenRow(Board Sudoku, int row)
        {
            //Check every square in the row except those with numbers
            //if an option is only found once
            //Set the square that contains it to that option
            byte[] timesFound = new byte[9];
            for (int i = 0; i < 9; i++) //for each square in this row
            {
                for (int j = 0; j < 9; j++) //for each item in it's options
                {
                    if (Sudoku.squareOptions[i + row * 9][j] == false) //if it is an option
                    {
                        timesFound[j]++; //add one to it's times found
                    }
                }
            }
            bool[] foundArray = new bool[] { true, true, true, true, true, true, true, true, true };
            for (byte number = 0; number < 9; number++) //for each number in timesFound
            {
                if (timesFound[number] == 1) //if it was only found once in this row
                {
                    foundArray[number] = false;
                    for (int square = 0; square < 9; square++) //for each square in this row
                    {
                        if (Sudoku.squareOptions[square + row * 9][number] == false) //this IS the square we're looking for
                        {
                            Sudoku.squareOptions[square + row * 9] = foundArray; //Set it options to only the Hidden single
                            MakeMove(Sudoku,square + row * 9, 0);
                            break;
                        }
                    }
                    break;
                }
            }
        }
        static void HiddenColumn(Board Sudoku, int column)
        {
            //Check every square in the Column except those with numbers
            //if an option is only foun once
            //Set the square that contains it to that option
            byte[] timesFound = new byte[9];
            for (int i = 0; i < 9; i++) //for each square in this column
            {
                for (int j = 0; j < 9; j++) //for each item in it's options
                {
                    if (Sudoku.squareOptions[i * 9 + column][j] == false) //if it is an option
                    {
                        timesFound[j]++; //add one to it's times found
                    }
                }
            }
            bool[] foundArray = new bool[] { true, true, true, true, true, true, true, true, true };
            for (byte number = 0; number < 9; number++) //for each number in timesFound
            {
                if (timesFound[number] == 1) //if it was only found once in this column
                {
                    foundArray[number] = false;
                    for (int square = 0; square < 9; square++) //for each square in this column
                    {
                        if (Sudoku.squareOptions[column + square * 9][number] == false) //this IS the square we're looking for
                        {
                            Sudoku.squareOptions[column + square * 9] = foundArray;
                            MakeMove(Sudoku, column + square* 9, 0);
                            break;
                        }
                    }
                    break;
                }
            }
        }
        static void HiddenBox(Board Sudoku, int box)
        {
            //Check every square in the Column except those with numbers
            //if an option is only foun once
            //Set the square that contains it to that option
            byte[] timesFound = new byte[9];
            byte startingSquare = (byte)((box / 3 * 27) + (box % 3)*3);
            for (int i = 0; i < 3; i++) //for each row each row in this box
            {
                for (int k = 0; k < 3; k++) //for each square in this row
                {
                    for (int j = 0; j < 9; j++) //for each item in it's options
                    {
                        if (Sudoku.squareOptions[startingSquare + 9 * i + k][j] == false) //if it is an option
                        {
                            timesFound[j]++; //add one to it's times found
                        }
                    }
                }
            }
            bool[] foundArray = new bool[] { true, true, true, true, true, true, true, true, true };
            for (byte number = 0; number < 9; number++) //for each number in timesFound
            {
                if (timesFound[number] == 1) //if it was only found once in this row
                {
                    foundArray[number] = false;
                    for (int square = 0; square < 3; square++) //for each row each row in this box
                    {
                        for (int k = 0; k < 3; k++) //for each square in this row
                        {
                            if (Sudoku.squareOptions[startingSquare + 9 * square + k][number] == false) //this IS the square we're looking for
                            {
                                Sudoku.squareOptions[startingSquare + 9 * square + k] = foundArray;
                                MakeMove(Sudoku, startingSquare + 9 * square + k, 0);
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }

        static void lockedBoxRow(Board Sudoku, int boxRow)
        {
            for (int box = 0; box < 3; box++) //For each box
            {
                for (int number = 0; number < 9; number++) //For each number
                {
                    int timesFound = 0;
                    byte rowFoundIn = 255;
                    for (byte row = 0; row < 3; row++) //For each row
                    {
                        for (int square = 0; square < 3; square++) //For each square in that row
                        {
                            if (Sudoku.squareOptions[boxRow * 27 + box * 3 + row * 9 + square][number] == false)
                            {
                                rowFoundIn = row;
                            }
                        }
                        if (rowFoundIn == row)
                        {
                            timesFound++;
                        }
                    }
                    if (timesFound == 1)
                    {
                        for (int i = 0; i < 3; i++) // for each box
                        {
                            if (i != box) // if its not the current box
                            {
                                for (int square = 0; square < 3; square++) // for each square in this box
                                {
                                    Sudoku.squareOptions[boxRow * 27 + rowFoundIn * 9 + i * 3 + square][number] = true;
                                    var possibleMoves =
                                        from x in Sudoku.squareOptions[boxRow * 27 + rowFoundIn * 9 + i * 3 + square]
                                        where x == false
                                        select x;
                                    if (possibleMoves.Count() == 1)
                                    {
                                        MakeMove(Sudoku, boxRow * 27 + rowFoundIn * 9 + i * 3 + square, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        static void lockedBoxColumn(Board Sudoku, int boxColumn)
        {
            for (int box = 0; box < 3; box++) //For each box
            {
                for (int number = 0; number < 9; number++) //For each number
                {
                    int timesFound = 0;
                    byte columnFoundIn = 255;
                    for (byte column = 0; column < 3; column++) //For each column
                    {
                        for (int square = 0; square < 3; square++) //For each square in that column
                        {
                            if (Sudoku.squareOptions[box * 27 + boxColumn * 3 + square * 9 + column][number] == false)
                            {
                                columnFoundIn = column;
                            }
                        }
                        if (columnFoundIn == column)
                        {
                            timesFound++;
                        }
                    }
                    if (timesFound == 1)
                    {
                        for (int i = 0; i < 3; i++) // for each box
                        {
                            if (i != box) // if its not the current box
                            {
                                for (int square = 0; square < 3; square++) // for each square in this box
                                {
                                    Sudoku.squareOptions[i * 27 + square * 9 + boxColumn * 3 + columnFoundIn][number] = true;
                                    var possibleMoves =
                                        from x in Sudoku.squareOptions[i * 27 + square * 9 + boxColumn * 3 + columnFoundIn]
                                        where x == false
                                        select x;
                                    if (possibleMoves.Count() == 1)
                                    {
                                        MakeMove(Sudoku, i * 27 + square * 9 + boxColumn * 3 + columnFoundIn, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static void MakeMove(Board Sudoku,int square, byte moveLevel)
        {
            // Cycle through the possible moves until the only one is found,
            // edit the board to contain this number
            ClearLines();
            //Get squares column row and box number
            byte column = (byte)square;
            byte row = 0;
            byte box = 0;
            for (; column > 8; column -= 9)
            {
                row++;
            }
            box += (byte)((column / 3 * 3) + (row / 3 * 27));
            for (int option = 0; option < 9; option++)
            {
                if (Sudoku.squareOptions[square][option] == false)
                {
                    Console.Write("Square {0} found to be {1}", square, option+1);
                    Sudoku.squares[square] = (byte)(option + 1);
                    for (int x = 0; x < 9; x++)
                    { Sudoku.squareOptions[square][x] = true; }
                    Display((byte)(option + 1), square);
                    moveLevel = 0;
                    lockedLevel = 0;
                    //Remove move from adjacent squares, if this makes any new moves possible then make them
                    if (autoSolve == false)
                    {
                        Console.ReadKey(true);
                    }
                    for (int j = 0; j < 9; j++)
                    {
                        //Row
                        Sudoku.squareOptions[row * 9 + j][option] = true; 
                        var possibleMoves =
                            (from x in Sudoku.squareOptions[row * 9 + j]
                            where x == false
                            select x).Count();
                        if (possibleMoves == 1)
                        {
                            MakeMove(Sudoku, row * 9 + j, moveLevel);
                        }
                        //Column
                        Sudoku.squareOptions[9 * j + column][option] = true; 
                        possibleMoves =
                            (from x in Sudoku.squareOptions[9 * j + column]
                            where x == false
                            select x).Count();
                        if (possibleMoves == 1)
                        {
                            MakeMove(Sudoku, 9 * j + column, moveLevel);
                        }
                        //box
                        Sudoku.squareOptions[box + (j/3*9) + (j % 3)][option] = true;
                        possibleMoves =
                            (from x in Sudoku.squareOptions[box + (j / 3 * 9) + (j % 3)]
                            where x == false
                            select x).Count();
                        if (possibleMoves == 1)
                        {
                            MakeMove(Sudoku, box + (j / 3 * 9) + (j % 3), moveLevel);
                        }
                    }
                    break;
                };
            }
            
        }

        static void UIinit()
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
        static void ClearLines() {
            Console.SetCursorPosition(0, 16);
            for (int i = 0; i < 10; i++)
			{
                Console.Write("                                                  ");
            }
            Console.SetCursorPosition(0, 16);
        }
        static void Display(byte number, int location)
        {
            int column = location % 9;
            int row = (location - column) / 9;
            //Account for gaps in board
            if (row > 2)
            {
                if (row > 5)
                {
                    row++;
                }
                row++;
            }
            if (column > 2)
            {
                if (column > 5)
                {
                    column += 3;
                }
                column += 3;
            }
            Console.SetCursorPosition(17 + column, 3 + row);
            if (number == 0)
            {
                Console.Write(" ");
            }
            else
            {
                Console.Write(number);
            }
            Console.SetCursorPosition(0, 21);
        }
    }
    public class Board
    {
        //VARIABLES
        //The number in each square - 0=blank
        public byte[] squares = new byte[81];
        //squareoptions[squarenumber] contains false for each possible number
        public bool[][] squareOptions = new bool[81][];

        public byte EmptySquares()
        {
            byte currentMoves = 0;
            foreach (byte square in squares)
            {
                if (square == 0)
                {
                    currentMoves++;
                }
            }
            return currentMoves;
        }
        public void init()
        {
            for (int i = 0; i < 81; i++)
            {
                squareOptions[i] = new bool[9];
            }
        }
    }
}
