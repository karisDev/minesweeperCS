//history of games tbd...
using System;
namespace minesweeperCS
{
    class Frontend
    {
        public static string ascii = " __  __ _            ____\n|  \\/  (_)_ __   ___/ ___|_      _____  ___ _ __   ___ _ __\n| |\\/| | | '_ \\ / _ \\___ \\ \\ /\\ / / _ \\/ _ \\ '_ \\ / _ \\ '__|\n| |  | | | | | |  __/___) \\ V  V /  __/  __/ |_) |  __/ |\n|_|  |_|_|_| |_|\\___|____/ \\_/\\_/ \\___|\\___| .__/ \\___|_|\n                                           |_|";
        public static int userNumber(int lowest, int highest) // заставляет пользователя написать число в диапозоне lowest...highest, улучшенная функция из пред. проекта
        {
            int userAnswer;
            bool terminate;
            Console.Write("Input: ");
            terminate = Int32.TryParse(Console.ReadLine(), out userAnswer);
            while (!terminate || userAnswer < lowest || userAnswer > highest)
            {
                Console.Write("You should pick a number between " + lowest + "..." + highest + ": ");
                terminate = Int32.TryParse(Console.ReadLine(), out userAnswer);
            }
            return userAnswer;
        }
        public static bool Introduction() // Вступительное меню
        {
            ConsoleKeyInfo key;
            int userLine = 1;
            bool terminate = false;
            while (!terminate)
            {
                Console.Clear();
                Console.WriteLine(ascii);
                Console.WriteLine("Choose an action:");
                switch (userLine)
                {
                    case 0:
                        Console.Write("New Game\nSettings\n--> Exit");
                        userLine = 3;
                        break;
                    case 1:
                        Console.Write("--> New Game\nSettings\nExit");
                        break;
                    case 2:
                        Console.Write("New Game\n--> Settings\nExit");
                        break;
                    case 3:
                        Console.Write("New Game\nSettings\n--> Exit");
                        break;
                    case 4:
                        Console.Write("--> New Game\nSettings\nExit");
                        userLine = 1;
                        break;
                }
                Console.WriteLine();
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        terminate = true;
                        break;
                    case ConsoleKey.UpArrow:
                        userLine--;
                        break;
                    case ConsoleKey.DownArrow:
                        userLine++;
                        break;
                }
            }
            switch (userLine)
            {
                case 1:
                    int[,] arr = Backend.fieldGenerator();
                    playOnField(arr);
                    Console.ReadKey();
                    return false;
                case 2:
                    Settings();
                    return false;
                case 3: //exit
                    return true;
                default:
                    return false;
            }
        }
        static void playOnField(int[,] arr) // Астрологи объявили неделю говнокода! Я постараюсь переписать и разделить по функциям, а пока простите....
        {
            int bombsReallyLeft = Backend.bombsNumber; // настоящее число бомб, может не совпадать
            int bombsLeft = Backend.bombsNumber; // количество оставшихся бомб может не совпадать с количеством флажков
            int cellsLeft = (Backend.rowsNumber - 1) * (Backend.rowsNumber - 1);
            int userI = 0, userJ = 0;
            Backend.hiddenField[0, 0] = 3;
            bool loose = false;
            int flagOrOpen = 3; // 1 - Flag 2 - Open 3 - remove flag
            while (!loose)
            {
                Console.Clear();
                // параметры hiddenFiled 1 = закрыто, 2 = флаг, 3 = открыто
                switch (flagOrOpen)
                {
                    case 1: // Флаг
                        if (Backend.hiddenField[userI, userJ] != 1) // проверка если ячейка не закрытая, тогда ничего не делаем
                        {
                            break;
                        }
                        else
                        {
                            cellsLeft--;
                            Backend.hiddenField[userI, userJ] = 2;
                            if (arr[userI, userJ] == -1)
                            {
                                bombsReallyLeft--;
                            }
                        }
                        bombsLeft--;
                        break;
                    case 2: // открыть
                        if (arr[userI, userJ] == -1) // поражение при открытии бомбы
                        {
                            loose = true;
                            break;
                        }
                        if (Backend.hiddenField[userI, userJ] == 2) // открытие ячейки с флагом
                        {
                            Backend.hiddenField[userI, userJ] = 3;
                            bombsLeft++;
                        }
                        else if (Backend.hiddenField[userI, userJ] != 3)
                        {
                            Backend.hiddenField[userI, userJ] = 3;
                            cellsLeft--;
                        }
                        break;

                    case 3: // удалить флаг
                        if (Backend.hiddenField[userI, userJ] == 2)
                        {
                            Backend.hiddenField[userI, userJ] = 1;
                            cellsLeft++;
                            bombsLeft++;
                            if (arr[userI, userJ] == -1)
                            {
                                bombsReallyLeft++;
                            }
                        }
                        break;
                }
                Console.WriteLine("\n    \u25A0 - hidden symbol       V - flag          * - bomb        (num) - amount of bombs around\n");
                Console.Write("        ");
                for (int i = 1; i < Backend.rowsNumber; i++) // вывод номеров столбцов
                {
                    if (i < 10) { Console.Write(i + "  "); }
                    else { Console.Write(i + " "); }
                }
                Console.WriteLine("\n");
                for (int i = 1; i < Backend.rowsNumber; i++) // вывод остального поля, с номерами строк
                {
                    if (i < 10) { Console.Write("    " + i + ")  "); }
                    else { Console.Write("    " + i + ") "); }

                    for (int j = 1; j < Backend.rowsNumber; j++)
                    {
                        if (Backend.hiddenField[i, j] == 1 && loose != true)
                        {
                            Console.Write("\u25A0  ");
                        }
                        else if (Backend.hiddenField[i, j] == 2 && loose != true)
                        {
                            Console.Write("V  ");
                        }
                        else if (arr[i, j] != -1)
                        {
                            Console.Write(arr[i, j] + "  ");
                        }
                        else
                        {
                            Console.Write("*" + "  ");
                        }
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("\nUnoppened bombs: " + bombsLeft + "       Unoppened cells: " + cellsLeft);

                if (bombsLeft <= 0 || cellsLeft == bombsReallyLeft) // проверка выигрыша
                {
                    if (bombsReallyLeft == bombsLeft || (bombsReallyLeft == cellsLeft && Backend.bombsNumber == (bombsReallyLeft + bombsLeft)))
                    {
                        Console.Write("__   __           __        ___       _\n\\ \\ / /__  _   _  \\ \\      / (_)_ __ | |\n \\ V / _ \\| | | |  \\ \\ /\\ / /| | '_ \\| |\n  | | (_) | |_| |   \\ V  V / | | | | |_|\n  |_|\\___/ \\__,_|    \\_/\\_/  |_|_| |_(_)");
                        Console.Write("\nPress any key to go back to the menu...");
                        Console.ReadKey();
                        break;
                    }
                    else
                    {
                        Console.Write("Warning! You didn't find " + bombsReallyLeft + " bombs. Delete flags and check");
                        Console.ReadKey();
                    }
                }
                if (loose == true) // проверка проигрыша
                {
                    Console.WriteLine("\n     You've lost!");
                    Console.Write("     Press any key to go back to the menu...");
                    Console.ReadKey();
                    break;
                }
                Console.WriteLine("\n\nChoose a line:"); // выбор ячейки
                userI = userNumber(1, Backend.rowsNumber - 1);
                Console.WriteLine("\nChoose a coloumn:");
                userJ = userNumber(1, Backend.rowsNumber - 1);
                Console.WriteLine("\nWhat to do with cell (" + userI + "; " + userJ + ")?");
                Console.Write("1 - Flag\n2 - Open\n3 - Remove Flag or nothing\n");
                flagOrOpen = userNumber(1, 3);
            }
        }
        static void Settings() //Выбор количества бомб
        {
            ConsoleKeyInfo a;
            int userLine = 1;
            bool terminate = false;
            while (!terminate)
            {
                Console.Clear();
                Console.WriteLine(ascii);
                Console.WriteLine("Choose an option: ");
                switch (userLine)
                {
                    case 0:
                        Console.Write("Set default mode\nSet your own amount of rows and bombs\n-->Come back to the main screen\n");
                        userLine = 3;
                        break;
                    case 1:
                        Console.Write("--> Set default mode\nSet your own amount of rows and bombs\nCome back to the main screen\n");
                        break;
                    case 2:
                        Console.Write("Set default mode\n--> Set your own amount of rows and bombs\nCome back to the main screen\n");
                        break;
                    case 3:
                        Console.Write("Set default mode\nSet your own amount of rows and bombs\n--> Come back to the main screen\n");
                        break;
                    case 4:
                        Console.Write("--> Set default mode\nSet your own amount of rows and bombs\nCome back to the main screen\n");
                        userLine = 1;
                        break;
                }
                a = Console.ReadKey();
                switch (a.Key)
                {
                    case ConsoleKey.Enter:
                        terminate = true;
                        break;
                    case ConsoleKey.UpArrow:
                        userLine--;
                        break;
                    case ConsoleKey.DownArrow:
                        userLine++;
                        break;
                }
            }
            switch (userLine)
            {
                case 1:
                    defaultSettings();
                    return;
                case 2:
                    Console.WriteLine("\nChoose the amount of rows: ");
                    Backend.rowsNumber = Frontend.userNumber(3, 20) + 1;
                    Console.WriteLine("\nChoose amount of bombs: ");
                    Backend.bombsNumber = Frontend.userNumber(1, ((Backend.rowsNumber - 1) * (Backend.rowsNumber - 1)) - 1);
                    return;
                case 3:
                    return;
            }
        }
        static void defaultSettings() // Выбор стандартных настроек
        {
            ConsoleKeyInfo a;
            int userLine = 1;
            bool terminate = false;
            while (!terminate)
            {
                Console.Clear();
                Console.WriteLine(ascii);
                Console.WriteLine("Choose an action:");
                switch (userLine)
                {
                    case 0:
                        Console.Write("Easy mode (9 rows and 10 bombs)\nNormal mode (16 rows and 40 bombs)\n--> Hard mode (20 rows and 70 bombs)\n");
                        userLine = 3;
                        break;
                    case 1:
                        Console.Write("--> Easy mode (9 rows and 10 bombs)\nNormal mode (16 rows and 40 bombs)\nHard mode (20 rows and 70 bombs)\n");
                        break;
                    case 2:
                        Console.Write("Easy mode (9 rows and 10 bombs)\n--> Normal mode (16 rows and 40 bombs)\nHard mode (20 rows and 70 bombs)\n");
                        break;
                    case 3:
                        Console.Write("Easy mode (9 rows and 10 bombs)\nNormal mode (16 rows and 40 bombs)\n--> Hard mode (20 rows and 70 bombs)\n");
                        break;
                    case 4:
                        Console.Write("--> Easy mode (9 rows and 10 bombs)\nNormal mode (16 rows and 40 bombs)\nHard mode (20 rows and 70 bombs)\n");
                        userLine = 1;
                        break;
                }
                Console.WriteLine();
                a = Console.ReadKey();
                switch (a.Key)
                {
                    case ConsoleKey.Enter:
                        terminate = true;
                        break;
                    case ConsoleKey.UpArrow:
                        userLine--;
                        break;
                    case ConsoleKey.DownArrow:
                        userLine++;
                        break;
                }
            }
            Console.WriteLine("1) Easy mode (9 rows and 10 bombs)\n2) Normal mode (16 rows and 40 bombs)\n3) Hard mode (20 rows and 70 bombs)\n");
            switch (userLine)
            {
                case 1:
                    Backend.bombsNumber = 10;
                    Backend.rowsNumber = 10;
                    break;
                case 2:
                    Backend.bombsNumber = 40;
                    Backend.rowsNumber = 17;
                    break;
                case 3:
                    Backend.bombsNumber = 70;
                    Backend.rowsNumber = 21;
                    break;
            }
            return;
        }
    }
    class Backend
    {
        public static int[,] hiddenField; // 1 = закрыто, 2 = флаг, 3 = открыто
        public static int bombsNumber = 10;
        public static int rowsNumber = 10;
        public static int[,] fieldGenerator()
        {
            int bombsLeft = bombsNumber; // дублирование количества бомб
            int[,] arr = new int[rowsNumber + 1, rowsNumber + 1]; // массив с бомбами
            hiddenField = new int[rowsNumber + 1, rowsNumber + 1]; // статус каждого поля
            for (int i = 1; i < rowsNumber; i++) // вставляем n бомб в первые n ячеек массива, в остальные ячейки вставляем 0
            {
                for (int j = 1; j < rowsNumber; j++)
                {
                    if (bombsLeft > 0)
                    {
                        arr[i, j] = -1;
                        bombsLeft--;
                    }
                    else
                    {
                        arr[i, j] = 0;
                    }
                    hiddenField[i, j] = 1;
                }
            }

            Random rnd = new Random(); //шафлим массив
            for (int i = 0; i < 400; i++)
            {
                var rndInd1 = rnd.Next(1, rowsNumber);
                var rndInd2 = rnd.Next(1, rowsNumber);
                var rndInd3 = rnd.Next(1, rowsNumber);
                var rndInd4 = rnd.Next(1, rowsNumber);
                var temp = arr[rndInd1, rndInd3];
                arr[rndInd1, rndInd3] = arr[rndInd2, rndInd4];
                arr[rndInd2, rndInd4] = temp;
            }
            arr = numbersPutter(arr);
            return arr;
        }
        static int[,] numbersPutter(int[,] arr) // чекаем если есть бомбы вокруг
        {
            int bombsAroundCounter = 0;
            for (int i = 1; i < rowsNumber; i++)
            {
                for (int j = 1; j < rowsNumber; j++)
                {
                    if (arr[i, j] != -1)
                    {
                        if (arr[i - 1, j - 1] == -1)
                        {
                            bombsAroundCounter++;
                        }
                        if (arr[i, j - 1] == -1)
                        {
                            bombsAroundCounter++;
                        }
                        if (arr[i + 1, j - 1] == -1)
                        {
                            bombsAroundCounter++;
                        }
                        if (arr[i - 1, j] == -1)
                        {
                            bombsAroundCounter++;
                        }
                        if (arr[i + 1, j] == -1)
                        {
                            bombsAroundCounter++;
                        }
                        if (arr[i - 1, j + 1] == -1)
                        {
                            bombsAroundCounter++;
                        }
                        if (arr[i, j + 1] == -1)
                        {
                            bombsAroundCounter++;
                        }
                        if (arr[i + 1, j + 1] == -1)
                        {
                            bombsAroundCounter++;
                        }
                        arr[i, j] = bombsAroundCounter;
                    }
                    bombsAroundCounter = 0;
                }
            }
            return arr;
        }
    }
    class main
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool terminate = false;
            while (!terminate)
            {
                Console.Clear();
                terminate = Frontend.Introduction();
            }
            Console.Clear();
            Console.WriteLine(" ____                                                        _\n/ ___|  ___  ___   _   _  ___  _   _   ___  ___   ___  _ __ | |\n\\___ \\ / _ \\/ _ \\ | | | |/ _ \\| | | | / __|/ _ \\ / _ \\| '_ \\| |\n ___) |  __/  __/ | |_| | (_) | |_| | \\__ \\ (_) | (_) | | | |_|\n|____/ \\___|\\___|  \\__, |\\___/ \\__,_| |___/\\___/ \\___/|_| |_(_)\n                   |___/");
        }
    }
}
