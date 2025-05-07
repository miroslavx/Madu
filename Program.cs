using System;
using System.IO;

namespace Snake
{
    // Класс Program: инициализирует основные компоненты хранит глобальные настройки (цвета, звуки) и запускает главное меню.
    class Program
    {
        // Глобальные константы и настройки
        public const int MAP_WIDTH = 80;
        public const int MAP_HEIGHT = 25;
        public const string SCORES_FILENAME = "scores.txt";
        public const string SOUNDS_FOLDER_NAME = "Zvuki";

        public const char SNAKE_SYMBOL_CONST = '*';
        public const char FOOD_SYMBOL_CONST = '$';
        public const char SCISSORS_SYMBOL_CONST = 'X';
        public const char DYNAMIC_WALL_SYMBOL = '#';   

        // Глобальные цветовые схемы (могут изменяться через меню)
        public static ConsoleColor fgColorPermanentWall = ConsoleColor.Yellow;
        public static ConsoleColor fgColorDynamicWall = ConsoleColor.DarkGray;
        public static ConsoleColor fgColorSnake = ConsoleColor.Green;
        public static ConsoleColor fgColorFood = ConsoleColor.Red;
        public static ConsoleColor fgColorScissors = ConsoleColor.Cyan;
        public static ConsoleColor fgColorText = ConsoleColor.White;
        public static ConsoleColor bgColor = ConsoleColor.Black;

        public static Sounds gameSounds;

        static void Main(string[] args)
        {
            Console.Title = "Snake";
            Console.CursorVisible = false;
            try
            {
                Console.SetBufferSize(Math.Max(MAP_WIDTH, Console.WindowWidth), Math.Max(MAP_HEIGHT + 5, Console.WindowHeight));
                Console.SetWindowSize(MAP_WIDTH, MAP_HEIGHT + 5);
            }
            catch (IOException) { }
            catch (ArgumentOutOfRangeException) {  }


            string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SOUNDS_FOLDER_NAME);
            if (!Directory.Exists(soundPath))
            {
                Console.Error.WriteLine($"Hoiatus: Helide kausta '{soundPath}' ei leitud. Helid ei pruugi töötada.");
            }
            gameSounds = new Sounds(soundPath);

            bool exitGame = false;
            while (!exitGame)
            {
                Menu.ShowMainMenu(out exitGame);
            }

            Console.Clear();
            SetCurrentColors(fgColorText, bgColor);
            WriteTextAt("Mäng on lõppenud. Vajuta Enter väljumiseks...", (Console.BufferWidth / 2) - 20, Console.BufferHeight / 2);
            Console.ReadLine();
        }

        public static void SetCurrentColors(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        public static void WriteTextAt(string text, int x, int y)
        {
            if (x < 0) x = 0;
            if (y < 0) y = 0;

            // Простая проверка, чтобы x не выходил за левую границу буфера
            if (y >= Console.BufferHeight) y = Console.BufferHeight - 1;

            try
            {
                Console.SetCursorPosition(x, y);
                Console.Write(text); 
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        public static string GetSpeedDisplay(int currentDelay, int minDelay, int maxRealDelay)
        {
            if (maxRealDelay <= minDelay) return new string('+', 10);

            double speedFactor = (double)(maxRealDelay - currentDelay) / (maxRealDelay - minDelay);
            speedFactor = Math.Max(0, Math.Min(1, speedFactor));

            int plusCount = (int)(speedFactor * 9) + 1;
            return new string('+', plusCount);
        }
    }
}
