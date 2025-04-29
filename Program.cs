using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snake
{
    // Основной класс программы
    class Program
    {
        // Главный метод, точка входа в программу
        static void Main(string[] args)
        {
            // Установка размера буфера консоли
            Console.SetBufferSize(80, 25);
            // Скрытие курсора для более чистого отображения
            Console.CursorVisible = false;
            // Создание стен (рамки)
            Walls walls = new Walls(80, 25);
            // Отрисовка стен
            walls.Draw();
            // Определение начальной точки для хвоста змейки
            Point p = new Point(4, 5, '*');
            // Создание объекта змейки
            Snake snake = new Snake(p, 4, Direction.RIGHT);
            // Отрисовка начального состояния змейки
            snake.Draw();
            // Создание объекта для генерации еды
            FoodCreator foodCreator = new FoodCreator(80, 25, '$');
            // Создание первой порции еды
            Point food = foodCreator.CreateFood();
            // Отрисовка еды
            food.Draw();

            // Основной игровой цикл
            while (true)
            {
                // Проверка столкновения змейки со стенами или собственным хвостом
                if (walls.IsHit(snake) || snake.IsHitTail())
                {
                    break; // Если столкновение произошло, выходим из цикла (игра окончена)
                }

                // Проверка, съела ли змейка еду
                if(snake.Eat( food ) )
                {
                    // Если съела, создаем новую порцию еды
                    food = foodCreator.CreateFood();
                    // Отрисовываем новую еду
                    food.Draw();
                }
                else
                {
                    // Если не съела, просто двигаем змейку
                    snake.Move();
                }
                // Небольшая пауза для контроля скорости игры
                Thread.Sleep(100);
                // Проверка, была ли нажата клавиша
                if (Console.KeyAvailable)
                {
                    // Считываем нажатую клавишу без отображения её на экране
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    // Передаем нажатую клавишу змейке для обработки
                    snake.HandleKey(keyInfo.Key);
                }
            }
            // Вывод сообщения "Игра Окончена" после завершения цикла
            WriteGameOver();
            // Ожидание нажатия Enter перед закрытием консоли
            Console.ReadLine();
        }

        //Конец игры
        static void WriteGameOver()
        {
            int xOffset = 25;
            int yOffset = 8; 
            Console.ForegroundColor = ConsoleColor.Red; 
            Console.SetCursorPosition(xOffset, yOffset++); 
            WriteText("============================", xOffset, yOffset++); 
            WriteText("        И Г Р А    О К О Н Ч Е Н А", xOffset + 1, yOffset++);
            WriteText("============================", xOffset, yOffset++); 
        }
        static void WriteText( String text, int xOffset, int yOffset )
        {
            Console.SetCursorPosition(xOffset, yOffset); 
            Console.WriteLine( text );                    
        }
    }
}
