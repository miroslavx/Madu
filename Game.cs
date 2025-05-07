using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq; // Для Any()

namespace Snake
{
    // Класс Game: Содержит логику классического игрового режима 'Змейки'.
    static class Game
    {
        // Константы для классического режима
        const int INITIAL_SNAKE_LENGTH = 4;
        const int INITIAL_GAME_DELAY_MS = 150;
        const int MIN_GAME_DELAY_MS = 60;
        const int DELAY_DECREASE_STEP = 10;
        const int INITIAL_FOOD_COUNT = 1;

        const int SCISSORS_SPAWN_FOOD_COUNT_MIN = 5;
        const int SCISSORS_SPAWN_FOOD_COUNT_MAX = 10;
        const int DYNAMIC_WALL_LENGTH = 15; 
        const int POINTS_PER_DYNAMIC_WALL_SPAWN = 10; 
        static int foodEatenSinceLastScissors = 0;
        static int nextScissorsSpawnTarget = 0;
        static Random random = new Random();


        public static void PlayClassicGame()
        {
            Console.BackgroundColor = Program.bgColor;
            Console.Clear();
            Console.CursorVisible = false;

            Walls walls = new Walls(Program.MAP_WIDTH, Program.MAP_HEIGHT);
            Program.SetCurrentColors(Program.fgColorPermanentWall, Program.bgColor);
            walls.DrawPermanentWalls();

            Point snakeStartPos = new Point(Program.MAP_WIDTH / 4, Program.MAP_HEIGHT / 2, Program.SNAKE_SYMBOL_CONST);
            Snake snake = new Snake(snakeStartPos, INITIAL_SNAKE_LENGTH, Direction.RIGHT);
            Program.SetCurrentColors(Program.fgColorSnake, Program.bgColor);
            snake.Draw();

            FoodCreator foodCreator = new FoodCreator(Program.MAP_WIDTH, Program.MAP_HEIGHT, Program.FOOD_SYMBOL_CONST);
            ScissorsCreator scissorsCreator = new ScissorsCreator(Program.MAP_WIDTH, Program.MAP_HEIGHT, Program.SCISSORS_SYMBOL_CONST);
            Point currentScissors = null;

            ResetScissorsSpawnTarget();
            walls.ClearDynamicObstacles();

            List<Point> foodItems = new List<Point>();
            for (int i = 0; i < INITIAL_FOOD_COUNT; i++)
            {
                foodItems.Add(foodCreator.CreateFood(snake.GetBody(), currentScissors, walls.GetDynamicObstacles()));
            }
            Program.SetCurrentColors(Program.fgColorFood, Program.bgColor);
            foreach (var food in foodItems) food.Draw();

            int score = 0;
            int gameDelayMs = INITIAL_GAME_DELAY_MS;
            int lastScoreForWallSpawn = 0;

            while (true)
            {
                Console.SetCursorPosition(0, Program.MAP_HEIGHT + 1);
                Program.SetCurrentColors(Program.fgColorText, Program.bgColor);
                string speedDisplay = Program.GetSpeedDisplay(gameDelayMs, MIN_GAME_DELAY_MS, 200); // 200 - условная макс. задержка для отображения
                Console.Write($"Skoor: {score}   Kiirus: {speedDisplay.PadRight(10)}                 "); // Пробелы для очистки предыдущей строки

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Escape) break;
                    snake.HandleKey(keyInfo.Key);
                }

                if (walls.IsHit(snake) || snake.IsHitTail())
                {
                    Program.gameSounds?.PlayGameOver();
                    break;
                }

                bool ateFoodThisTurn = false;

                for (int i = foodItems.Count - 1; i >= 0; i--) // Обратный цикл для безопасного удаления
                {
                    Program.SetCurrentColors(Program.fgColorSnake, Program.bgColor); // Цвет змеи перед попыткой съесть
                    if (snake.Eat(foodItems[i])) //рисует съеденную еду символом змеи
                    {
                        Program.gameSounds?.PlayEat();
                        score++;
                        ateFoodThisTurn = true;
                        foodItems.RemoveAt(i);

                        foodEatenSinceLastScissors++;
                        if (gameDelayMs > MIN_GAME_DELAY_MS)
                        {
                            gameDelayMs = Math.Max(MIN_GAME_DELAY_MS, gameDelayMs - DELAY_DECREASE_STEP);
                        }

                        Program.SetCurrentColors(Program.fgColorFood, Program.bgColor); // Цвет еды перед созданием новой
                        foodItems.Add(foodCreator.CreateFood(snake.GetBody(), currentScissors, walls.GetDynamicObstacles()));
                        foodItems.Last().Draw();
                        break;
                    }
                }

                // Динамические стены
                if (score / POINTS_PER_DYNAMIC_WALL_SPAWN > lastScoreForWallSpawn / POINTS_PER_DYNAMIC_WALL_SPAWN && score > 0)
                {
                    lastScoreForWallSpawn = score;
                    walls.ClearDynamicObstacles(); // Очищаем старые
                    Figure newObstacle = CreateRandomObstacle(
                        Program.MAP_WIDTH, Program.MAP_HEIGHT, DYNAMIC_WALL_LENGTH, Program.DYNAMIC_WALL_SYMBOL,
                        snake.GetBody(), foodItems, currentScissors, walls.GetDynamicObstacles()
                    );
                    if (newObstacle != null)
                    {
                        walls.AddObstacle(newObstacle);
                        Program.SetCurrentColors(Program.fgColorDynamicWall, Program.bgColor);
                        newObstacle.Draw();
                    }
                }

                // Ножницы
                if (currentScissors == null && foodEatenSinceLastScissors >= nextScissorsSpawnTarget)
                {
                    Program.SetCurrentColors(Program.fgColorScissors, Program.bgColor);
                    currentScissors = scissorsCreator.CreateScissors(snake.GetBody(), foodItems.Any() ? foodItems[0] : null, walls.GetDynamicObstacles());
                    currentScissors?.Draw();
                    ResetScissorsSpawnTarget(); // Сброс счетчика для следующих ножниц
                }

                if (currentScissors != null && snake.HitScissors(currentScissors))
                {
                    snake.ShortenSnake(); 
                    Program.SetCurrentColors(Program.fgColorText, Program.bgColor); // Для очистки символа ножниц
                    Console.BackgroundColor = Program.bgColor;
                    currentScissors.Clear();
                    currentScissors = null;
                    // Замедление после ножниц
                    gameDelayMs = Math.Min(INITIAL_GAME_DELAY_MS, (int)(gameDelayMs * 1.25));
                }

                if (!ateFoodThisTurn)
                {
                    Program.SetCurrentColors(Program.fgColorSnake, Program.bgColor);
                    snake.Move();
                }

                Thread.Sleep(gameDelayMs);
            }

            // Конец игры
            Console.CursorVisible = false;
            Program.SetCurrentColors(ConsoleColor.Red, Program.bgColor); // Цвет для "GAME OVER"
            Program.WriteTextAt("MÄNG LÄBI!", (Console.BufferWidth / 2) - 5, Program.MAP_HEIGHT / 2 - 1);
            Program.WriteTextAt($"Sinu skoor: {score}", (Console.BufferWidth / 2) - 7, Program.MAP_HEIGHT / 2 + 0);

            Menu.SaveScore(score, "Klassika"); // Сохранение счета
            Program.SetCurrentColors(Program.fgColorText, Program.bgColor);
            Program.WriteTextAt("Vajuta Enter menüüsse naasmiseks...", (Console.BufferWidth / 2) - 17, Program.MAP_HEIGHT / 2 + 6); // Ниже чем SaveScore
            Console.CursorVisible = true; 
            Console.ReadLine();
            Console.CursorVisible = false;
        }

        static void ResetScissorsSpawnTarget()
        {
            foodEatenSinceLastScissors = 0;
            nextScissorsSpawnTarget = random.Next(SCISSORS_SPAWN_FOOD_COUNT_MIN, SCISSORS_SPAWN_FOOD_COUNT_MAX + 1);
        }

        static Figure CreateRandomObstacle(int mapW, int mapH, int length, char sym, List<Point> snakeBody, List<Point> foodItems, Point currentScissors, List<Figure> existingObstacles)
        {
            for (int attempts = 0; attempts < 20; attempts++) // Больше попыток для длинной стены
            {
                bool isHorizontal = random.Next(0, 2) == 0;
                Figure newObstacle;

                if (isHorizontal)
                {
                    int y = random.Next(2, mapH - 2); // Не на самом краю
                    int startX = random.Next(1, mapW - 1 - length); 
                    startX = Math.Max(1, startX); // Не левее 1
                    newObstacle = new HorizontalLine(startX, startX + length - 1, y, sym);
                }
                else // Вертикальная
                {
                    int x = random.Next(2, mapW - 2); // Не на самом краю
                    int startY = random.Next(1, mapH - 1 - length);
                    startY = Math.Max(1, startY);
                    newObstacle = new VerticalLine(startY, startY + length - 1, x, sym);
                }

                bool collisionDetected = false;
                foreach (Point pObs in newObstacle.GetPoints())
                {
                    if (snakeBody.Any(ps => ps.IsHit(pObs))) { collisionDetected = true; break; }
                    if (foodItems.Any(pf => pf.IsHit(pObs))) { collisionDetected = true; break; }
                    if (currentScissors != null && currentScissors.IsHit(pObs)) { collisionDetected = true; break; }
                    if (existingObstacles.Any(eo => eo.ContainsPoint(pObs))) { collisionDetected = true; break; }
                }
                if (!collisionDetected) return newObstacle;
            }
            return null; 
        }
    }
}