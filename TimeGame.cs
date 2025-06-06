using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;


namespace Snake
{
    // Класс TimeGame: Змейка на время
    static class TimeGame
    {
        const int INITIAL_SNAKE_LENGTH = 3;           
        const int INITIAL_GAME_DELAY_MS = 80;       
        const int MIN_GAME_DELAY_MS = 30;             
        const int DELAY_DECREASE_STEP_ON_EAT = 3;     
        const int INITIAL_FOOD_COUNT = 30;            
        const int TIME_ATTACK_DURATION_SECONDS = 30;  

        public static void PlayTimeAttackGame()
        {
            Console.BackgroundColor = Program.bgColor;
            Console.Clear();
            Console.CursorVisible = false;

            // Инициализация игрового поля, змейки и еды
            Walls walls = new Walls(Program.MAP_WIDTH, Program.MAP_HEIGHT);
            Program.SetCurrentColors(Program.fgColorPermanentWall, Program.bgColor);
            walls.DrawPermanentWalls(); // В этом режиме только рамка

            Point snakeStartPos = new Point(Program.MAP_WIDTH / 4, Program.MAP_HEIGHT / 2, Program.SNAKE_SYMBOL_CONST);
            Snake snake = new Snake(snakeStartPos, INITIAL_SNAKE_LENGTH, Direction.RIGHT);
            Program.SetCurrentColors(Program.fgColorSnake, Program.bgColor);
            snake.Draw();

            FoodCreator foodCreator = new FoodCreator(Program.MAP_WIDTH, Program.MAP_HEIGHT, Program.FOOD_SYMBOL_CONST);

            List<Point> foodItems = new List<Point>();
            for (int i = 0; i < INITIAL_FOOD_COUNT; i++)
            {
                // В TimeAttack нет динамических стен или ножниц для проверки при создании еды
                foodItems.Add(foodCreator.CreateFood(snake.GetBody(), null, null));
            }
            Program.SetCurrentColors(Program.fgColorFood, Program.bgColor);
            foreach (var food in foodItems) food.Draw();

            int score = 0;
            int gameDelayMs = INITIAL_GAME_DELAY_MS;

            Stopwatch gameTimer = new Stopwatch();
            gameTimer.Start();

            bool allFoodEaten = false;

            // Основной игровой цикл
            while (true)
            {
                // Проверка оставшегося времени
                long timeLeftMs = (TIME_ATTACK_DURATION_SECONDS * 1000) - gameTimer.ElapsedMilliseconds;
                if (timeLeftMs <= 0)
                {
                    timeLeftMs = 0;
                    gameTimer.Stop(); // Остановить таймер, если время вышло
                    Program.gameSounds?.PlayGameOver();
                    break;
                }

                // Отображение счета, скорости и оставшегося времени
                Console.SetCursorPosition(0, Program.MAP_HEIGHT + 1);
                Program.SetCurrentColors(Program.fgColorText, Program.bgColor);
                string speedDisplay = Program.GetSpeedDisplay(gameDelayMs, MIN_GAME_DELAY_MS, 150); // 150 - условная макс. задержка для TimeAttack
                Console.Write($"Skoor: {score}   Kiirus: {speedDisplay.PadRight(10)}   Aeg: {timeLeftMs / 1000:D2}s  ");
                Console.Write("            "); // Очистка остатка строки

                // Обработка ввода пользователя (выход, управление змейкой)
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Escape) { gameTimer.Stop(); break; }
                    snake.HandleKey(keyInfo.Key);
                }

                // Проверка столкновений змейки со стенами или хвостом
                if (walls.IsHit(snake) || snake.IsHitTail())
                {
                    gameTimer.Stop();
                    Program.gameSounds?.PlayGameOver();
                    break;
                }

                bool ateFoodThisTurn = false;

                // Логика поедания еды и обновления счета/скорости
                for (int i = foodItems.Count - 1; i >= 0; i--)
                {
                    Program.SetCurrentColors(Program.fgColorSnake, Program.bgColor);
                    if (snake.Eat(foodItems[i]))
                    {
                        Program.gameSounds?.PlayEat();
                        score++;
                        ateFoodThisTurn = true;
                        foodItems.RemoveAt(i);

                        if (gameDelayMs > MIN_GAME_DELAY_MS)
                        {
                            gameDelayMs = Math.Max(MIN_GAME_DELAY_MS, gameDelayMs - DELAY_DECREASE_STEP_ON_EAT);
                        }

                        if (foodItems.Count == 0)
                        {
                            allFoodEaten = true;
                            gameTimer.Stop(); // Остановить таймер
                            break;
                        }
                        break;
                    }
                }
                if (allFoodEaten) break; // Выход из основного цикла, если вся еда съедена


                // Движение змейки, если еда не была съедена
                if (!ateFoodThisTurn)
                {
                    Program.SetCurrentColors(Program.fgColorSnake, Program.bgColor);
                    snake.Move();
                }

                Thread.Sleep(gameDelayMs);
            }

            // Логика завершения игры: отображение результатов, сохранение счета
            Console.CursorVisible = false;
            Program.SetCurrentColors(ConsoleColor.Red, Program.bgColor);
            Program.WriteTextAt("MÄNG LÄBI!", (Console.BufferWidth / 2) - 5, Program.MAP_HEIGHT / 2 - 1);
            if (allFoodEaten && gameTimer.ElapsedMilliseconds < TIME_ATTACK_DURATION_SECONDS * 1000)
            {
                Program.WriteTextAt("KÕIK TOIDUD SÖÖDUD!", (Console.BufferWidth / 2) - 10, Program.MAP_HEIGHT / 2 + 0);
                Program.WriteTextAt($"Sinu skoor: {score}", (Console.BufferWidth / 2) - 7, Program.MAP_HEIGHT / 2 + 1);
            }
            else
            {
                Program.WriteTextAt($"Sinu skoor: {score}", (Console.BufferWidth / 2) - 7, Program.MAP_HEIGHT / 2 + 0);
            }

            Menu.SaveScore(score, "Aeg"); // Сохранение счета с тегом режима

            Program.SetCurrentColors(Program.fgColorText, Program.bgColor);
            Program.WriteTextAt("Vajuta Enter menüüsse naasmiseks...", (Console.BufferWidth / 2) - 17, Program.MAP_HEIGHT / 2 + 6);
            Console.CursorVisible = true;
            Console.ReadLine();
            Console.CursorVisible = false;
        }
    }
}