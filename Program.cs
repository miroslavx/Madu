using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Diagnostics; 
using System.Linq; // Добавлен для Any()

namespace Snake
{
    // Класс Program: Основной класс игры, содержит главный цикл, логику игры, меню и управление состояниями.
    class Program
    {
        const int MAP_WIDTH = 80;
        const int MAP_HEIGHT = 25;
        const string SCORES_FILENAME = "scores.txt";
        const string SOUNDS_FOLDER_NAME = "Zvuki";
        public const char SNAKE_SYMBOL_CONST = '*';
        const char FOOD_SYMBOL_CONST = '$';
        const char SCISSORS_SYMBOL_CONST = 'X';
        const char DYNAMIC_WALL_SYMBOL = '#';
        const int INITIAL_SNAKE_LENGTH = 4;
        const int SCISSORS_SPAWN_FOOD_COUNT_MIN = 5;
        const int SCISSORS_SPAWN_FOOD_COUNT_MAX = 10;
        const int DYNAMIC_WALL_LENGTH = 15; // Увеличена длина стены
        const int POINTS_PER_DYNAMIC_WALL = 10;

        static ConsoleColor fgColorPermanentWall = ConsoleColor.Yellow; // Цвет для рамки
        static ConsoleColor fgColorDynamicWall = ConsoleColor.DarkGray;
        static ConsoleColor fgColorSnake = ConsoleColor.Green;
        static ConsoleColor fgColorFood = ConsoleColor.Red;
        static ConsoleColor fgColorScissors = ConsoleColor.Cyan;
        static ConsoleColor fgColorText = ConsoleColor.White;
        public static ConsoleColor bgColor = ConsoleColor.Black;

        static Sounds gameSounds;
        static Random random = new Random();
        static int foodEatenSinceLastScissors = 0;
        static int nextScissorsSpawnTarget = 0;
        
        static void Main(string[] args)
        {
            Console.Title = "Snake";
            Console.CursorVisible = false;
            try { Console.SetBufferSize(Math.Max(MAP_WIDTH, 80), Math.Max(MAP_HEIGHT + 5, 30)); }
            catch { /* Игнор */ }

            string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SOUNDS_FOLDER_NAME);
            if (!Directory.Exists(soundPath))
            {
                 Console.Error.WriteLine($"Hoiatus: Helide kausta '{soundPath}' ei leitud. Helid ei pruugi töötada.");
            }
            gameSounds = new Sounds(soundPath);

            bool exitGame = false;
            while (!exitGame)
            {
                ShowMainMenu(out exitGame);
            }

            Console.Clear();
            SetCurrentColors(fgColorText, bgColor);
            Console.WriteLine("Mäng on lõppenud. Vajuta Enter väljumiseks...");
            Console.ReadLine();
        }

        static void ShowMainMenu(out bool exitSelected)
        {
            exitSelected = false;
            gameSounds.PlayBackground();

            Console.BackgroundColor = bgColor;
            Console.Clear();
            SetCurrentColors(fgColorText, bgColor);

            int centerAlignX = (Console.BufferWidth / 2);

            WriteTextAt("╔════════════════════════╗", centerAlignX - 14, 3);
            WriteTextAt("║        S N A K E       ║", centerAlignX - 14, 4);
            WriteTextAt("╚════════════════════════╝", centerAlignX - 14, 5);

            string[] menuItems = {
                "1. Alusta mängu (Klassikaline)",
                "2. Mäng ajale (Kiire)",
                "3. Rekordid",
                "4. Värvivalik",
                "5. Välju"
            };
            int menuY = 8;
            int menuItemWidth = 30; 
            foreach (string item in menuItems)
            {
                WriteTextAt($"   {item.PadRight(menuItemWidth-6)}", centerAlignX - menuItemWidth / 2, menuY++);
            }
            WriteTextAt("".PadRight(menuItemWidth, '-'), centerAlignX - menuItemWidth/2, menuY++);

            Console.SetCursorPosition(Math.Max(0,centerAlignX - 8), menuY +1);
            Console.Write("Vali number: ");

            Console.CursorVisible = true;
            string choice = Console.ReadLine();
            Console.CursorVisible = false;

            switch (choice)
            {
                case "1":
                    gameSounds.StopBackground();
                    PlayGame(GameMode.Classic);
                    break;
                case "2":
                    gameSounds.StopBackground();
                    PlayGame(GameMode.TimeAttack);
                    break;
                case "3":
                    ShowHighScores();
                    break;
                case "4":
                    SelectColorScheme();
                    break;
                case "5":
                    exitSelected = true;
                    gameSounds.StopBackground();
                    break;
                default:
                    Console.SetCursorPosition(0, menuY + 3);
                    Console.WriteLine("Vale valik. Vajuta Enter...");
                    Console.ReadLine();
                    break;
            }
        }

        enum GameMode { Classic, TimeAttack }

        static void PlayGame(GameMode mode)
        {
            Console.BackgroundColor = bgColor;
            Console.Clear();
            Console.CursorVisible = false;

            Walls walls = new Walls(MAP_WIDTH, MAP_HEIGHT);
            SetCurrentColors(fgColorPermanentWall, bgColor); // Цвет для рамки
            walls.DrawPermanentWalls();

            Point snakeStartPos = new Point(MAP_WIDTH / 4, MAP_HEIGHT / 2, SNAKE_SYMBOL_CONST);
            Snake snake = new Snake(snakeStartPos, INITIAL_SNAKE_LENGTH, Direction.RIGHT);
            SetCurrentColors(fgColorSnake, bgColor);
            snake.Draw();

            FoodCreator foodCreator = new FoodCreator(MAP_WIDTH, MAP_HEIGHT, FOOD_SYMBOL_CONST);
            Point currentScissors = null;
            ScissorsCreator scissorsCreator = null;

            if (mode == GameMode.Classic)
            {
                 scissorsCreator = new ScissorsCreator(MAP_WIDTH, MAP_HEIGHT, SCISSORS_SYMBOL_CONST);
                 ResetScissorsSpawnTarget();
            }
            
            walls.ClearDynamicObstacles(); // Очищаем динамические стены от предыдущей игры

            List<Point> foodItems = new List<Point>();
            int initialFoodCount = (mode == GameMode.TimeAttack) ? 20 : 1; // Больше еды для TimeAttack

            for (int i = 0; i < initialFoodCount; i++)
            {
                foodItems.Add(foodCreator.CreateFood(snake.GetBody(), currentScissors, walls.GetDynamicObstacles()));
            }
            SetCurrentColors(fgColorFood, bgColor);
            foreach (var food in foodItems) food.Draw();


            int score = 0;
            int gameDelayMs = (mode == GameMode.TimeAttack) ? 100 : 150; // Быстрее в TimeAttack
            int minDelayMs = (mode == GameMode.TimeAttack) ? 50 : 60;   // И мин. задержка ниже
            int delayDecrease = 10;
            int lastScoreForWall = 0;

            Stopwatch gameTimer = null;
            int timeAttackDurationSeconds = 30; // Уменьшено время
            if (mode == GameMode.TimeAttack)
            {
                gameTimer = new Stopwatch();
                gameTimer.Start();
            }

            while (true)
            {
                Console.SetCursorPosition(0, MAP_HEIGHT + 1);
                SetCurrentColors(fgColorText, bgColor);
                string speedDisplay = GetSpeedDisplay(gameDelayMs, minDelayMs, 200);
                Console.Write($"Skoor: {score}   Kiirus: {speedDisplay.PadRight(10)}");

                if (mode == GameMode.TimeAttack && gameTimer != null)
                {
                    long timeLeftMs = (timeAttackDurationSeconds * 1000) - gameTimer.ElapsedMilliseconds;
                    if (timeLeftMs <= 0) { timeLeftMs = 0; gameSounds.PlayGameOver(); break; }
                    Console.Write($"   Aeg: {timeLeftMs / 1000:D2}s  "); // :D2 для двузначного отображения секунд
                }
                Console.Write("            ");


                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Escape) break;
                    snake.HandleKey(keyInfo.Key);
                }
                
                if (walls.IsHit(snake) || snake.IsHitTail())
                {
                    if (mode == GameMode.TimeAttack && gameTimer != null) gameTimer.Stop();
                    gameSounds.PlayGameOver();
                    break;
                }
                
                bool ateFoodThisTurn = false;
                
                SetCurrentColors(fgColorSnake, bgColor);
                for (int i = foodItems.Count -1; i >= 0; i--)
                {
                    if (snake.Eat(foodItems[i]))
                    {
                        gameSounds.PlayEat();
                        score++;
                        ateFoodThisTurn = true;
                        foodItems.RemoveAt(i);

                        if (mode == GameMode.Classic)
                        {
                            foodEatenSinceLastScissors++;
                             if (gameDelayMs > minDelayMs)
                            {
                                gameDelayMs = Math.Max(minDelayMs, gameDelayMs - delayDecrease);
                            }
                            SetCurrentColors(fgColorFood, bgColor);
                            foodItems.Add(foodCreator.CreateFood(snake.GetBody(), currentScissors, walls.GetDynamicObstacles()));
                            foodItems.Last().Draw();
                        }
                        else 
                        {
                            if(!foodItems.Any()) // Если вся еда съедена в TimeAttack
                            {
                                gameTimer?.Stop();
                                Console.Write(" Kõik toidud söödud!"); 
                                gameSounds.PlayEat(); 
                                Thread.Sleep(1500); 
                                break; 
                            }
                        }
                        break; 
                    }
                }
                // Выход из цикла while(true) если break был в цикле по еде (для TimeAttack)
                if(mode == GameMode.TimeAttack && !foodItems.Any() && (gameTimer == null || !gameTimer.IsRunning)) break;


                if (mode == GameMode.Classic)
                {
                    if (score / POINTS_PER_DYNAMIC_WALL > lastScoreForWall / POINTS_PER_DYNAMIC_WALL && score > 0)
                    {
                        lastScoreForWall = score;
                        walls.ClearDynamicObstacles();
                        Figure newObstacle = CreateRandomObstacle(MAP_WIDTH, MAP_HEIGHT, DYNAMIC_WALL_LENGTH, DYNAMIC_WALL_SYMBOL, snake.GetBody(), foodItems, currentScissors, walls.GetDynamicObstacles());
                        if (newObstacle != null)
                        {
                            walls.AddObstacle(newObstacle);
                            SetCurrentColors(fgColorDynamicWall, bgColor);
                            newObstacle.Draw();
                        }
                    }
                    
                    if (scissorsCreator !=null && currentScissors == null && foodEatenSinceLastScissors >= nextScissorsSpawnTarget)
                    {
                        SetCurrentColors(fgColorScissors, bgColor);
                        currentScissors = scissorsCreator.CreateScissors(snake.GetBody(), foodItems.Any() ? foodItems[0] : null, walls.GetDynamicObstacles());
                        currentScissors?.Draw();
                        ResetScissorsSpawnTarget();
                    }
                    if (currentScissors != null && snake.HitScissors(currentScissors))
                    {
                        snake.ShortenSnake();
                        SetCurrentColors(fgColorText, bgColor); 
                        Console.BackgroundColor = bgColor;
                        currentScissors.Clear();
                        currentScissors = null;
                        gameDelayMs = Math.Min(200, (int)(gameDelayMs * 1.25));
                    }
                }


                if (!ateFoodThisTurn)
                {
                    SetCurrentColors(fgColorSnake, bgColor);
                    snake.Move();
                }
                
                Thread.Sleep(gameDelayMs);
            }

            Console.CursorVisible = false;
            SetCurrentColors(ConsoleColor.Red, bgColor);
            WriteTextAt("MÄNG LÄBI!", (Console.BufferWidth / 2) - 5, MAP_HEIGHT / 2 -1); // Чуть выше
            WriteTextAt($"Sinu skoor: {score}", (Console.BufferWidth / 2) - 7, MAP_HEIGHT / 2 + 0);
            
            SaveScore(score, (mode == GameMode.TimeAttack ? "Aeg" : "Klassika"));
            SetCurrentColors(fgColorText, bgColor);
            WriteTextAt("Vajuta Enter menüüsse naasmiseks...", (Console.BufferWidth/2)-17, MAP_HEIGHT/2+2);
            Console.CursorVisible = true;
            Console.ReadLine();
            Console.CursorVisible = false;
        }

        static Figure CreateRandomObstacle(int mapW, int mapH, int length, char sym, List<Point> snake, List<Point> food, Point scissors, List<Figure> existingObstacles)
        {
            for (int attempts = 0; attempts < 15; attempts++) // Больше попыток для длинной стены
            {
                bool isHorizontal = random.Next(0, 2) == 0;
                Figure newObstacle;
                
                if (isHorizontal)
                {
                    int y = random.Next(2, mapH - 2);
                    int startX = random.Next(1, mapW - length); // Убедимся, что стена помещается
                    if (startX + length -1 >= mapW -1) startX = mapW - 1 - length; // Коррекция, если выходит за правую границу
                    startX = Math.Max(1, startX); // Не левее 1
                    newObstacle = new HorizontalLine(startX, startX + length - 1, y, sym);
                }
                else
                {
                    int x = random.Next(2, mapW - 2);
                    int startY = random.Next(1, mapH - length);
                    if (startY + length - 1 >= mapH -1) startY = mapH - 1 - length;
                    startY = Math.Max(1, startY);
                    newObstacle = new VerticalLine(startY, startY + length - 1, x, sym);
                }
                
                bool collision = false;
                foreach(Point pObs in newObstacle.GetPoints())
                {
                    if(snake.Any(ps => ps.IsHit(pObs))) { collision = true; break; }
                    if(food.Any(pf => pf.IsHit(pObs))) { collision = true; break; }
                    if(scissors != null && scissors.IsHit(pObs)) { collision = true; break; }
                    if(existingObstacles.Any(eo => eo.ContainsPoint(pObs))) {collision = true; break;} // Проверка с другими динамическими стенами
                }
                if (!collision) return newObstacle;
            }
            return null;
        }

        static string GetSpeedDisplay(int currentDelay, int minDelay, int maxRealDelay)
        {
            // maxRealDelay - это значение gameDelayMs, когда скорость минимальна (например, 200 или 250)
            // minDelay - это gameDelayMs, когда скорость максимальна (например, 50 или 60)
            if (maxRealDelay <= minDelay) return "+"; 
            
            // Переворачиваем: чем меньше currentDelay, тем ВЫШЕ значение для отображения плюсов
            // (maxRealDelay - currentDelay) даст нам "насколько мы быстрее минимальной скорости"
            double speedFactor = (double)(maxRealDelay - currentDelay) / (maxRealDelay - minDelay);
            
            int plusCount = (int)(speedFactor * 9) + 1; 
            plusCount = Math.Max(1, Math.Min(10, plusCount));
            return new string('+', plusCount);
        }


        static void ResetScissorsSpawnTarget()
        {
            foodEatenSinceLastScissors = 0;
            nextScissorsSpawnTarget = random.Next(SCISSORS_SPAWN_FOOD_COUNT_MIN, SCISSORS_SPAWN_FOOD_COUNT_MAX + 1);
        }

        static void SaveScore(int score, string gameModeTag = "")
        {
            Console.CursorVisible = true;
            SetCurrentColors(fgColorText, bgColor);
            Console.Write("Sisesta oma nimi (min 3 tähte): ");
            string playerName = "";
            while (true)
            {
                playerName = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(playerName) && playerName.Length >= 3) break;
                Console.Write("Nimi peab olema vähemalt 3 tähte. Proovi uuesti: ");
            }
            Console.CursorVisible = false;

            try
            {
                using (StreamWriter sw = new StreamWriter(SCORES_FILENAME, true))
                {
                    string tag = string.IsNullOrEmpty(gameModeTag) ? "" : $"[{gameModeTag}] ";
                    sw.WriteLine($"{tag}{playerName}:{score}");
                }
                Console.WriteLine("Skoor salvestatud!");
            }
            catch (Exception ex) { Console.WriteLine($"Viga skoori salvestamisel: {ex.Message}"); }
        }

        static void ShowHighScores()
        {
            Console.BackgroundColor = bgColor;
            Console.Clear();
            SetCurrentColors(fgColorText, bgColor);
            
            int centerAlignX = (Console.BufferWidth / 2);
            WriteTextAt("╔════════════════════════╗", centerAlignX - 14, 1);
            WriteTextAt("║       R E K O R D I D    ║", centerAlignX - 14, 2);
            WriteTextAt("╚════════════════════════╝", centerAlignX - 14, 3);
            Console.WriteLine();

            if (!File.Exists(SCORES_FILENAME)) { Console.WriteLine("Rekordeid veel pole."); }
            else
            {
                List<ScoreEntry> scores = new List<ScoreEntry>();
                try
                {
                    string[] lines = File.ReadAllLines(SCORES_FILENAME);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length == 2 && int.TryParse(parts[1], out int scoreValue))
                        {
                            scores.Add(new ScoreEntry(parts[0], scoreValue));
                        }
                    }
                    for (int i = 0; i < scores.Count - 1; i++)
                    {
                        for (int j = 0; j < scores.Count - i - 1; j++)
                        {
                            if (scores[j].Score < scores[j + 1].Score)
                            {
                                ScoreEntry temp = scores[j]; scores[j] = scores[j + 1]; scores[j + 1] = temp;
                            }
                        }
                    }
                    int count = 0;
                    int scoreListY = 5; // Начальная Y позиция для списка рекордов
                    foreach (ScoreEntry entry in scores)
                    {
                        // Выравниваем вывод рекордов
                        string scoreLine = $"{count + 1}. {entry.Name} - {entry.Score}";
                        WriteTextAt(scoreLine, centerAlignX - (scoreLine.Length / 2), scoreListY++);
                        count++;
                        if (count >= 10) break;
                    }
                    if (count == 0) WriteTextAt("Rekordeid veel pole.", centerAlignX -10, scoreListY);
                }
                catch (Exception ex) { WriteTextAt($"Viga rekordite lugemisel: {ex.Message}",0 ,15); }
            }
            WriteTextAt("Vajuta Enter menüüsse naasmiseks...", centerAlignX - 17, Math.Max(Console.CursorTop, MAP_HEIGHT-2));
            Console.CursorVisible = true;
            Console.ReadLine();
            Console.CursorVisible = false;
        }

        static void SelectColorScheme()
        {
            Console.BackgroundColor = bgColor;
            Console.Clear();
            SetCurrentColors(fgColorText, bgColor);
            
            int centerAlignX = (Console.BufferWidth / 2);
            WriteTextAt("╔════════════════════════╗", centerAlignX - 14, 3);
            WriteTextAt("║     V Ä R V I V A L I K  ║", centerAlignX - 14, 4);
            WriteTextAt("╚════════════════════════╝", centerAlignX - 14, 5);


            string[] schemes = {
                "1. Standard", 
                "2. Monokroomne Roheline", 
                "3. Monokroomne Punane", 
                "4. Monokroomne Kollane"
            };
            int schemeY = 7;
            int schemeItemWidth = 30;

            foreach(string s in schemes)
            {
                WriteTextAt(s, centerAlignX - schemeItemWidth/2, schemeY++);
            }
            WriteTextAt("Sinu valik: ", centerAlignX - schemeItemWidth/2, schemeY + 1);
            
            Console.CursorVisible = true;
            string choice = Console.ReadLine();
            Console.CursorVisible = false;

            switch (choice)
            {
                case "1": // Стандарт
                    fgColorPermanentWall = ConsoleColor.Yellow; fgColorDynamicWall = ConsoleColor.DarkGray;
                    fgColorSnake = ConsoleColor.Green; fgColorFood = ConsoleColor.Red;
                    fgColorScissors = ConsoleColor.Cyan; fgColorText = ConsoleColor.White;
                    bgColor = ConsoleColor.Black;
                    break;
                case "2": // Монохром Зеленый
                    fgColorPermanentWall = ConsoleColor.Green; fgColorDynamicWall = ConsoleColor.DarkGreen;
                    fgColorSnake = ConsoleColor.Green; fgColorFood = ConsoleColor.Green;
                    fgColorScissors = ConsoleColor.Green; fgColorText = ConsoleColor.Green;
                    bgColor = ConsoleColor.Black;
                    break;
                case "3": // Монохром Красный
                    fgColorPermanentWall = ConsoleColor.Red; fgColorDynamicWall = ConsoleColor.DarkRed;
                    fgColorSnake = ConsoleColor.Red; fgColorFood = ConsoleColor.Red;
                    fgColorScissors = ConsoleColor.Red; fgColorText = ConsoleColor.Red;
                    bgColor = ConsoleColor.Black;
                    break;
                case "4": // Монохром Желтый
                    fgColorPermanentWall = ConsoleColor.Yellow; fgColorDynamicWall = ConsoleColor.DarkYellow;
                    fgColorSnake = ConsoleColor.Yellow; fgColorFood = ConsoleColor.Yellow;
                    fgColorScissors = ConsoleColor.Yellow; fgColorText = ConsoleColor.Yellow;
                    bgColor = ConsoleColor.Black;
                    break;
                default: Console.WriteLine("Vale valik. Värve ei muudetud."); break;
            }
            Console.BackgroundColor = bgColor;
            Console.Clear(); // Очищаем с новым фоном, чтобы надпись была на нем
            SetCurrentColors(fgColorText, bgColor);
            WriteTextAt("Värviskeem rakendatud. Vajuta Enter...", centerAlignX-18, 15);
            Console.CursorVisible = true;
            Console.ReadLine();
            Console.CursorVisible = false;
        }

        static void SetCurrentColors(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        static void WriteTextAt(string text, int x, int y)
        {
             if (y >= 0 && y < Console.BufferHeight && x >= 0 && x < Console.BufferWidth)
            {
                try { Console.SetCursorPosition(x, y); Console.WriteLine(text); } // WriteLine для переноса, если надо
                catch (ArgumentOutOfRangeException) { /* Игнор */ }
            } else if (y >=0 && y < Console.BufferHeight) { // если Х вне, но Y в норме, пробуем писать с Х=0
                 try { Console.SetCursorPosition(0, y); Console.WriteLine(text); }
                 catch (ArgumentOutOfRangeException) { /* Игнор */ }
            }
        }
    }
}
