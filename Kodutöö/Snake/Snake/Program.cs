// Program.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO; // Для работы с файлами

namespace Snake
{
    // Структура для хранения рекордов
    struct ScoreEntry : IComparable<ScoreEntry>
    {
        public string Name;
        public int Score;

        // Для сортировки по убыванию очков
        public int CompareTo(ScoreEntry other)
        {
            return other.Score.CompareTo(this.Score);
        }
    }

    // Основной класс программы
    class Program
    {
        // --- Настройки игры ---
        const int MAP_WIDTH = 80;
        const int MAP_HEIGHT = 25;
        const string SCORE_FILE = "Nimed.txt";
        const char SCISSORS_SYMBOL = 'X';
        const int SCISSORS_SPAWN_CHANCE = 7;
        const int MAX_SCORES_TO_SHOW = 10;

        // --- Переменные состояния игры ---
        static int score = 0;
        static int gameSpeedDelay = 150;
        static int minSpeedDelay = 40;
        static int speedIncreaseStep = 12;
        static Point scissors = null;
        static Random random = new Random();

        // --- Объекты игры ---
        static Sounds sounds;
        static FoodCreator foodCreator;
        static ScissorsCreator scissorsCreator;
        static Snake snake;
        static Walls walls;
        static Point food = null;

        // --- Цветовые схемы ---
        static ConsoleColor wallColor = ConsoleColor.Gray;
        static ConsoleColor snakeColor = ConsoleColor.Green;
        static ConsoleColor foodColor = ConsoleColor.Red;
        static ConsoleColor textColor = ConsoleColor.White;
        static ConsoleColor backgroundColor = ConsoleColor.Black;
        static ConsoleColor scissorsColor = ConsoleColor.Cyan;


        // Главный метод, точка входа в программу
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Ussimäng";

            SelectColorScheme();
            DisplayHighScores(true);

            bool playAgain = true;
            while (playAgain)
            {
                InitializeGame();
                GameLoop();
                GameOver();
                playAgain = AskPlayAgain();
            }

            Console.Clear();
            Console.WriteLine("Aitäh mängimast! Vajuta Enter väljumiseks...");
            Console.ReadLine();
        }

        // Инициализация или сброс состояния для новой игры
        static void InitializeGame()
        {
            score = 0;
            gameSpeedDelay = 150;
            scissors = null;
            food = null;

            try
            {
                int consoleHeight = MAP_HEIGHT + 5;
                if (OperatingSystem.IsWindows())
                {
                    Console.SetWindowSize(MAP_WIDTH, consoleHeight);
                    Console.SetBufferSize(MAP_WIDTH, consoleHeight);
                }
                else
                {
                    Console.SetBufferSize(MAP_WIDTH, consoleHeight);
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"Hoiatus: Konsooli suuruse määramine ebaõnnestus: {ex.Message}");
                Console.WriteLine("Vajuta Enter jätkamiseks...");
                Console.ReadLine();
            }

            Console.CursorVisible = false;
            ApplyColorScheme();

            // Создаем объект Sounds ПЕРЕД его использованием
            sounds = new Sounds();

            // Запускаем фоновую музыку
            sounds?.PlayBackgroundMusic(); // Используем ?. на случай, если sounds не создался

            walls = new Walls(MAP_WIDTH, MAP_HEIGHT);
            walls.Draw(wallColor, backgroundColor);

            Point startPoint = new Point(MAP_WIDTH / 4, MAP_HEIGHT / 2, '*');
            snake = new Snake(startPoint, 4, Direction.RIGHT);
            snake.Draw(snakeColor, backgroundColor);

            foodCreator = new FoodCreator(MAP_WIDTH, MAP_HEIGHT, '$');
            scissorsCreator = new ScissorsCreator(MAP_WIDTH, MAP_HEIGHT, SCISSORS_SYMBOL);

            food = foodCreator.CreateFood(snake.pList, scissors);
            food.Draw(foodColor, backgroundColor);

            DisplayScore();
        }


        // Основной игровой цикл
        static void GameLoop()
        {
            while (true)
            {
                if (walls.IsHit(snake) || snake.IsHitTail())
                {
                    sounds?.PlayGameOverSound(); // Этот вызов теперь сам остановит фон
                    break;
                }

                Action redrawHead = () => snake.pList.LastOrDefault()?.Draw(snakeColor, backgroundColor);

                if (scissors != null && snake.HitScissors(scissors))
                {
                    // Тут можно добавить звук для ножниц, если он нужен
                    // sounds?.PlayScissorsSound(); // По аналогии с PlayEatSound
                    snake.ShortenSnake();
                    scissors.Clear();
                    scissors = null;
                    snake.Move();
                    redrawHead();
                }
                else if (food != null && snake.Eat(food))
                {
                    sounds?.PlayEatSound(); // Этот вызов теперь перезапустит фон
                    score++;
                    DisplayScore();

                    if (gameSpeedDelay > minSpeedDelay)
                    {
                        gameSpeedDelay -= speedIncreaseStep;
                        if (gameSpeedDelay < minSpeedDelay) gameSpeedDelay = minSpeedDelay;
                    }

                    food = foodCreator.CreateFood(snake.pList, scissors);
                    food?.Draw(foodColor, backgroundColor);

                    if (scissors == null && random.Next(0, SCISSORS_SPAWN_CHANCE) == 0)
                    {
                        scissors = scissorsCreator.CreateScissors(snake.pList, food);
                        scissors?.Draw(scissorsColor, backgroundColor);
                    }
                }
                else
                {
                    snake.Move();
                    redrawHead();
                }

                Thread.Sleep(gameSpeedDelay);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        sounds?.StopBackgroundMusic(); // Останавливаем музыку при выходе по Esc
                        break;
                    }
                    snake.HandleKey(keyInfo.Key);
                }
            } // Конец while(true)
        }

        // Завершение игры
        static void GameOver()
        {
            // Музыка уже остановлена в GameLoop при выходе из цикла
            // (либо вызовом PlayGameOverSound, либо StopBackgroundMusic при выходе по Esc)
            // sounds?.StopBackgroundMusic(); // Этот вызов здесь больше не нужен

            WriteGameOverMessage();
            AskNameAndSaveScore(score);
            DisplayHighScores(false);
        }


        // Отображение текущего счета и скорости
        static void DisplayScore()
        {
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
            int speedPercent = 0;
            int totalSpeedRange = 150 - minSpeedDelay;
            if (totalSpeedRange > 0)
            {
                int currentSpeedDecrease = 150 - gameSpeedDelay;
                speedPercent = (int)(((double)currentSpeedDecrease / totalSpeedRange) * 100);
            }
            string scoreText = $"Skoor: {score}    Kiirus: {speedPercent}%   ";
            WriteText(scoreText, 0, MAP_HEIGHT + 1);
        }


        // Вывод сообщения "Игра Окончена" по центру
        static void WriteGameOverMessage()
        {
            int xOffset = MAP_WIDTH / 2 - 15;
            int yOffset = MAP_HEIGHT / 2 - 2;
            ConsoleColor oldFg = Console.ForegroundColor;
            ConsoleColor oldBg = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = backgroundColor;
            ClearArea(xOffset - 1, yOffset, 32, 5);
            WriteText("===============================", xOffset, yOffset++);
            WriteText("   M Ä N G    L Ä B I   ", xOffset + 1, yOffset++);
            WriteText($"    Sinu skoor: {score}", xOffset + 1, yOffset++);
            WriteText("===============================", xOffset, yOffset++);
            Console.ForegroundColor = oldFg;
            Console.BackgroundColor = oldBg;
        }

        // Вспомогательный метод для вывода текста
        static void WriteText(String text, int xOffset, int yOffset)
        {
            if (xOffset >= 0 && xOffset < Console.BufferWidth && yOffset >= 0 && yOffset < Console.BufferHeight)
            {
                try
                {
                    Console.SetCursorPosition(xOffset, yOffset);
                    int len = text.Length;
                    int remainingWidth = Console.BufferWidth - xOffset;
                    if (len > remainingWidth) len = remainingWidth;
                    Console.Write(text.Substring(0, len).PadRight(remainingWidth));
                }
                catch (Exception ex) { DebugPrint($"WriteText Error: {ex.Message}"); }
            }
            else { DebugPrint($"WriteText Attempted write outside buffer: {xOffset},{yOffset}"); }
        }


        // --- Функции для таблицы рекордов ---
        static void AskNameAndSaveScore(int currentScore)
        {
            int inputYPos = MAP_HEIGHT / 2 + 3;
            int messageYPos = inputYPos + 1;
            ConsoleColor oldFg = Console.ForegroundColor;
            ConsoleColor oldBg = Console.BackgroundColor;
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
            string playerName = "";
            do
            {
                ClearArea(0, inputYPos, Console.BufferWidth, 2);
                WriteText("Sisesta oma nimi (vähemalt 3 tähte): ", 0, inputYPos);
                Console.SetCursorPosition("Sisesta oma nimi (vähemalt 3 tähte): ".Length, inputYPos);
                Console.CursorVisible = true;
                playerName = Console.ReadLine()?.Trim();
                Console.CursorVisible = false;

                if (string.IsNullOrWhiteSpace(playerName) || playerName.Length < 3)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    WriteText("Nimi peab olema vähemalt 3 tähte pikk. Proovi uuesti.", 0, messageYPos);
                    Console.ForegroundColor = textColor;
                    Thread.Sleep(1500);
                    playerName = null;
                }
            } while (string.IsNullOrWhiteSpace(playerName) || playerName.Length < 3);

            try
            {
                string fullPath = Path.GetFullPath(SCORE_FILE);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                File.AppendAllText(fullPath, $"{playerName}:{currentScore}{Environment.NewLine}");
                WriteText("Skoor salvestatud!", 0, messageYPos);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                WriteText($"Viga skoori salvestamisel: {ex.Message}", 0, messageYPos);
                Console.ForegroundColor = textColor;
            }
            finally
            {
                Console.ForegroundColor = oldFg;
                Console.BackgroundColor = oldBg;
            }
            WriteText("Vajuta Enter jätkamiseks...", 0, messageYPos + 1);
            Console.ReadKey(true);
        }

        static void DisplayHighScores(bool askToStartGame)
        {
            ApplyColorScheme();
            ConsoleColor headerColor = ConsoleColor.Yellow;
            ConsoleColor scoreColor = textColor;
            int maxEntriesToShow = MAX_SCORES_TO_SHOW;
            int headerHeight = 2;
            int footerHeight = askToStartGame ? 4 : 2;
            int tableWidth = 30;

            if (maxEntriesToShow + headerHeight + footerHeight > Console.WindowHeight)
            {
                maxEntriesToShow = Console.WindowHeight - headerHeight - footerHeight;
                if (maxEntriesToShow < 0) maxEntriesToShow = 0;
            }
            int totalTableHeight = headerHeight + maxEntriesToShow + footerHeight;
            int displayY = (Console.WindowHeight - totalTableHeight) / 2;
            if (displayY < 0) displayY = 0;
            int displayX = (MAP_WIDTH - tableWidth) / 2;
            if (displayX < 0) displayX = 0;

            Console.ForegroundColor = headerColor;
            WriteText("=== EDETABEL ===", displayX, displayY++);
            displayY++;

            Console.ForegroundColor = scoreColor;
            List<ScoreEntry> scores = LoadScores();
            scores.Sort();

            int count = 0;
            foreach (var entry in scores.Take(maxEntriesToShow))
            {
                string scoreLine = $"{(count + 1)}. {entry.Name} - {entry.Score}";
                if (scoreLine.Length > tableWidth - 2) scoreLine = scoreLine.Substring(0, tableWidth - 2) + "..";
                WriteText(scoreLine, displayX, displayY++);
                count++;
            }
            if (count == 0 && maxEntriesToShow > 0)
            {
                WriteText("Edetabel on tühi.", displayX, displayY++); count++;
            }
            while (count < maxEntriesToShow) { displayY++; count++; }

            displayY++;
            Console.ForegroundColor = textColor;

            if (askToStartGame)
            {
                WriteText("Vajuta Enter mängu alustamiseks...", displayX, displayY++);
                WriteText("Vajuta Esc väljumiseks...", displayX, displayY++);
                ConsoleKey key;
                do { key = Console.ReadKey(true).Key; } while (key != ConsoleKey.Enter && key != ConsoleKey.Escape);
                if (key == ConsoleKey.Escape) Environment.Exit(0);
                ApplyColorScheme();
            }
        }

        static List<ScoreEntry> LoadScores()
        {
            List<ScoreEntry> scores = new List<ScoreEntry>();
            string fullPath = Path.GetFullPath(SCORE_FILE);
            if (!File.Exists(fullPath)) { DebugPrint($"Score file not found: {fullPath}"); return scores; }
            try
            {
                string[] lines = File.ReadAllLines(fullPath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int scoreValue))
                    {
                        scores.Add(new ScoreEntry { Name = parts[0].Trim(), Score = scoreValue });
                    }
                    else { DebugPrint($"Invalid line in score file: '{line}'"); }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                WriteText($"Viga edetabeli laadimisel: {ex.Message}", 0, Console.WindowHeight - 1);
                Console.ResetColor(); Thread.Sleep(2000);
            }
            return scores;
        }

        // --- Функции для цветовых схем ---
        static void SelectColorScheme()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.WriteLine("Vali värviskeem:");
            Console.WriteLine("1. Standardne (Valge mustal)");
            Console.WriteLine("2. Roheline monitor (Roheline mustal)");
            Console.WriteLine("3. Punane valgel");
            Console.WriteLine("4. Inverteeritud (Must valgel)");

            ConsoleKeyInfo keyInfo;
            bool selectionMade = false;
            while (!selectionMade)
            {
                keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        wallColor = ConsoleColor.Gray; snakeColor = ConsoleColor.Green;
                        foodColor = ConsoleColor.Red; textColor = ConsoleColor.White;
                        backgroundColor = ConsoleColor.Black; scissorsColor = ConsoleColor.Cyan;
                        selectionMade = true; break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        wallColor = ConsoleColor.DarkGreen; snakeColor = ConsoleColor.Green;
                        foodColor = ConsoleColor.Green; textColor = ConsoleColor.Green;
                        backgroundColor = ConsoleColor.Black; scissorsColor = ConsoleColor.Green;
                        selectionMade = true; break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        wallColor = ConsoleColor.DarkRed; snakeColor = ConsoleColor.Red;
                        foodColor = ConsoleColor.DarkBlue; textColor = ConsoleColor.DarkRed;
                        backgroundColor = ConsoleColor.White; scissorsColor = ConsoleColor.Blue;
                        selectionMade = true; break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        wallColor = ConsoleColor.DarkGray; snakeColor = ConsoleColor.Black;
                        foodColor = ConsoleColor.DarkRed; textColor = ConsoleColor.Black;
                        backgroundColor = ConsoleColor.White; scissorsColor = ConsoleColor.Black;
                        selectionMade = true; break;
                }
            }
        }

        static void ApplyColorScheme()
        {
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
            try { Console.Clear(); } catch (IOException) { }
        }

        // --- Вспомогательные функции ---
        static void ClearArea(int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0) return;
            string emptyLine = new string(' ', width);
            ConsoleColor currentBg = Console.BackgroundColor;
            Console.BackgroundColor = backgroundColor;
            for (int i = 0; i < height; i++)
            {
                int currentY = y + i;
                if (currentY >= 0 && currentY < Console.BufferHeight && x >= 0)
                {
                    int effectiveWidth = Math.Min(width, Console.BufferWidth - x);
                    if (effectiveWidth > 0)
                    {
                        try
                        {
                            Console.SetCursorPosition(x, currentY);
                            Console.Write(emptyLine.Substring(0, effectiveWidth));
                        }
                        catch (Exception ex) { DebugPrint($"ClearArea Error: {ex.Message}"); }
                    }
                }
            }
            Console.BackgroundColor = currentBg;
        }

        static bool AskPlayAgain()
        {
            int yPos = Console.WindowHeight - 2;
            if (yPos < MAP_HEIGHT + 5) yPos = MAP_HEIGHT + 5;
            if (yPos >= Console.BufferHeight) yPos = Console.BufferHeight - 1;
            Console.SetCursorPosition(0, yPos);
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
            WriteText("Mängida uuesti? (Jah - Enter / Ei - Esc)", 0, yPos);
            ConsoleKey key;
            do { key = Console.ReadKey(true).Key; } while (key != ConsoleKey.Enter && key != ConsoleKey.Escape);
            return key == ConsoleKey.Enter;
        }

        static void DebugPrint(string message)
        {
            // Раскомментируй для отладки проблем со звуком или путем к файлам
            // try {
            //     Console.SetCursorPosition(0, Console.WindowHeight - 1);
            //     ConsoleColor oldFg = Console.ForegroundColor;
            //     ConsoleColor oldBg = Console.BackgroundColor;
            //     Console.ForegroundColor = ConsoleColor.Yellow;
            //     Console.BackgroundColor = ConsoleColor.Black; // Или другой фон для видимости
            //     WriteText($"DEBUG: {message}", 0, Console.WindowHeight - 1);
            //     Console.ForegroundColor = oldFg;
            //     Console.BackgroundColor = oldBg;
            // } catch {} // Игнорируем ошибки вывода отладки
        }

    } // Конец класса Program
} // Конец namespace Snake