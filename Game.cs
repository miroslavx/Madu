using System;
using System.Collections.Generic;
using System.Threading;


namespace Snake
{
    // Класс Game: Содержит логику классического игрового режима 'Змейки'.
    static class Game
    {
        // Настройки для классического режима
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
            // Создание игрового окружения
            Console.BackgroundColor = Program.bgColor;
            Console.Clear();
            Console.CursorVisible = false;

            // Создание стен, змейки, еды и ножниц
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

            ResetScissorsSpawnTarget(); // Установка начального порога для появления ножниц
            walls.ClearDynamicObstacles(); // Очистка возможных предыдущих динамических стен

            // Создание начальной еды
            List<Point> foodItems = new List<Point>();
            for (int i = 0; i < INITIAL_FOOD_COUNT; i++)
            {
                foodItems.Add(foodCreator.CreateFood(snake.GetBody(), currentScissors, walls.GetDynamicObstacles()));
            }
            Program.SetCurrentColors(Program.fgColorFood, Program.bgColor);
            foreach (var food in foodItems) food.Draw();

            int score = 0;
            int gameDelayMs = INITIAL_GAME_DELAY_MS;
            int lastScoreForWallSpawn = 0; // Для отслеживания появления динамических стен

            // Игровой цикл
            while (true)
            {
                // Отображение информации (счет, скорость)
                Console.SetCursorPosition(0, Program.MAP_HEIGHT + 1);
                Program.SetCurrentColors(Program.fgColorText, Program.bgColor);
                string speedDisplay = Program.GetSpeedDisplay(gameDelayMs, MIN_GAME_DELAY_MS, 200); // 200 - условная макс. задержка
                Console.Write($"Skoor: {score}   Kiirus: {speedDisplay.PadRight(10)}                 ");

                // Обработка ввода пользователя
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Escape) break; // Выход из игры по Escape
                    snake.HandleKey(keyInfo.Key);
                }

                // Проверка столкновений (стены, хвост)
                if (walls.IsHit(snake) || snake.IsHitTail())
                {
                    Program.gameSounds?.PlayGameOver();
                    break; // Конец игры
                }

                bool ateFoodThisTurn = false;

                // Логика поедания еды
                for (int i = foodItems.Count - 1; i >= 0; i--) // Обратный цикл для безопасного удаления из списка
                {
                    Program.SetCurrentColors(Program.fgColorSnake, Program.bgColor);
                    if (snake.Eat(foodItems[i])) // Метод Eat() также отрисовывает съеденный сегмент
                    {
                        Program.gameSounds?.PlayEat();
                        score++;
                        ateFoodThisTurn = true;
                        foodItems.RemoveAt(i); // Удаляем съеденную еду

                        foodEatenSinceLastScissors++; // Увеличиваем счетчик съеденной еды для ножниц
                        // Увеличение скорости игры
                        if (gameDelayMs > MIN_GAME_DELAY_MS)
                        {
                            gameDelayMs = Math.Max(MIN_GAME_DELAY_MS, gameDelayMs - DELAY_DECREASE_STEP);
                        }

                        // Создание новой еды
                        Program.SetCurrentColors(Program.fgColorFood, Program.bgColor);
                        Point newFood = foodCreator.CreateFood(snake.GetBody(), currentScissors, walls.GetDynamicObstacles());
                        foodItems.Add(newFood);
                        newFood.Draw(); 
                        break; // Выход из цикла, так как за один ход можно съесть одну еду
                    }
                }

                // Логика появления динамических стен
                if (score / POINTS_PER_DYNAMIC_WALL_SPAWN > lastScoreForWallSpawn / POINTS_PER_DYNAMIC_WALL_SPAWN && score > 0)
                {
                    lastScoreForWallSpawn = score;
                    walls.ClearDynamicObstacles(); // Очищаем старые динамические стены
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

                // Логика появления ножниц
                if (currentScissors == null && foodEatenSinceLastScissors >= nextScissorsSpawnTarget)
                {
                    Program.SetCurrentColors(Program.fgColorScissors, Program.bgColor);
                    Point firstFoodItem = null;
                    if (foodItems.Count > 0) 
                    {
                        firstFoodItem = foodItems[0];
                    }
                    currentScissors = scissorsCreator.CreateScissors(snake.GetBody(), firstFoodItem, walls.GetDynamicObstacles());
                    currentScissors?.Draw();
                    ResetScissorsSpawnTarget(); // Сброс счетчика для следующих ножниц
                }

                // Логика столкновения с ножницами
                if (currentScissors != null && snake.HitScissors(currentScissors))
                {
                    snake.ShortenSnake();
                    // Очистка символа ножниц с экрана
                    ConsoleColor originalBg = Console.BackgroundColor;
                    Console.BackgroundColor = Program.bgColor;
                    currentScissors.Clear();
                    Console.BackgroundColor = originalBg;
                    currentScissors = null;
                    // Замедление скорости после столкновения с ножницами
                    gameDelayMs = Math.Min(INITIAL_GAME_DELAY_MS, (int)(gameDelayMs * 1.25));
                }

                // Движение змейки, если еда не была съедена в этом ходу
                if (!ateFoodThisTurn)
                {
                    Program.SetCurrentColors(Program.fgColorSnake, Program.bgColor);
                    snake.Move();
                }

                Thread.Sleep(gameDelayMs); // Пауза для контроля скорости игры
            }

            // Отображение сообщения "Конец игры" и счета
            Console.CursorVisible = false;
            Program.SetCurrentColors(ConsoleColor.Red, Program.bgColor);
            Program.WriteTextAt("MÄNG LÄBI!", (Console.BufferWidth / 2) - 5, Program.MAP_HEIGHT / 2 - 1);
            Program.WriteTextAt($"Sinu skoor: {score}", (Console.BufferWidth / 2) - 7, Program.MAP_HEIGHT / 2 + 0);

            Menu.SaveScore(score, "Klassika"); // Сохранение счета

            // Приглашение вернуться в меню
            Program.SetCurrentColors(Program.fgColorText, Program.bgColor);
            Program.WriteTextAt("Vajuta Enter menüüsse naasmiseks...", (Console.BufferWidth / 2) - 17, Program.MAP_HEIGHT / 2 + 6);
            Console.CursorVisible = true;
            Console.ReadLine();
            Console.CursorVisible = false;
        }

        // Сбрасывает счетчик съеденной еды и устанавливает новый порог для появления ножниц.
        static void ResetScissorsSpawnTarget()
        {
            foodEatenSinceLastScissors = 0;
            nextScissorsSpawnTarget = random.Next(SCISSORS_SPAWN_FOOD_COUNT_MIN, SCISSORS_SPAWN_FOOD_COUNT_MAX + 1);
        }

        // Создает случайное препятствие (стену) на игровом поле.
        static Figure CreateRandomObstacle(int mapW, int mapH, int length, char sym, List<Point> snakeBody, List<Point> foodItems, Point currentScissors, List<Figure> existingObstacles)
        {
            for (int attempts = 0; attempts < 20; attempts++) // Несколько попыток для размещения стены
            {
                bool isHorizontal = random.Next(0, 2) == 0; // Случайный выбор ориентации стены
                Figure newObstacle;

                // Создание горизонтальной или вертикальной стены
                if (isHorizontal)
                {
                    int y = random.Next(2, mapH - 2); // y-координата (не у самых краев)
                    int startX = random.Next(1, mapW - 1 - length);
                    startX = Math.Max(1, startX); // Гарантия, что стена не выходит за левый край
                    newObstacle = new HorizontalLine(startX, startX + length - 1, y, sym);
                }
                else // Вертикальная стена
                {
                    int x = random.Next(2, mapW - 2); // x-координата (не у самых краев)
                    int startY = random.Next(1, mapH - 1 - length);
                    startY = Math.Max(1, startY); // Гарантия, что стена не выходит за верхний край
                    newObstacle = new VerticalLine(startY, startY + length - 1, x, sym);
                }

                // Проверка на столкновение нового препятствия с существующими объектами
                bool collisionDetected = false;
                foreach (Point pObs in newObstacle.GetPoints())
                {
                    // Проверка столкновения с телом змейки
                    if (snakeBody != null)
                    {
                        foreach (Point ps in snakeBody)
                        {
                            if (ps.IsHit(pObs)) { collisionDetected = true; break; }
                        }
                        if (collisionDetected) break;
                    }

                    // Проверка столкновения с едой
                    if (foodItems != null)
                    {
                        foreach (Point pf in foodItems)
                        {
                            if (pf.IsHit(pObs)) { collisionDetected = true; break; }
                        }if (collisionDetected) break;
                    }

                    // Проверка столкновения с ножницами
                    if (currentScissors != null && currentScissors.IsHit(pObs)) { collisionDetected = true; break; }
                    // Проверка столкновения с другими существующими препятствиями
                    if (existingObstacles != null)
                    {
                        foreach (Figure eo in existingObstacles)
                        {
                            if (eo.ContainsPoint(pObs)) { collisionDetected = true; break; }
                        }
                        if (collisionDetected) break;
                    }
                }

                if (!collisionDetected) return newObstacle; // Если столкновений нет, возвращаем созданное препятствие
            }
            return null; // Если не удалось разместить препятствие без столкновений
        }
    }
}