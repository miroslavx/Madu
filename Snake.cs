using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс Snake: Представляет змейку, управляет ее движением, ростом и взаимодействием с другими объектами.
    class Snake : Figure
    {
        Direction direction;
        const int MIN_SNAKE_LENGTH_AFTER_SCISSORS = 3;

        public Snake(Point initialPos, int length, Direction initialDirection)
        {
            direction = initialDirection;
            pList = new List<Point>();
            // Тело змейки
            for (int i = 0; i < length; i++)
            {
                Point p = new Point(initialPos);
                p.Move(i, direction);
                pList.Add(p);
            }
        }

        public List<Point> GetBody()
        {
            return pList;
        }

        //Движения
        internal void Move()
        {
            if (pList == null || pList.Count == 0) return; // Проверка что вообще змейка существует

            Point tail = new Point(pList[0]); // Получаем хвост (первый элемент)
            ConsoleColor originalBg = Console.BackgroundColor;
            Console.BackgroundColor = Program.bgColor;
            tail.Clear(); // Стираем хвост с предыдущей позиции
            Console.BackgroundColor = originalBg;

            pList.RemoveAt(0); // Удаляем хвост из списка
            Point head = GetNextPointPosition(); // Получаем новую позицию головы
            pList.Add(head); // Добавляем голову в список
            head.Draw(); // Отрисовываем новую голову
        }

        // съела ли змейка еду
        internal bool Eat(Point food)
        {
            Point nextHead = GetNextPointPosition();
            if (nextHead.IsHit(food))
            {
                food.sym = Program.SNAKE_SYMBOL_CONST; // Символ еды меняется на символ змейки
                pList.Add(food); // Добавляем еду как новую часть головы (рост змейки)
                food.Draw();
                return true;
            }
            return false;
        }

        // Столкновение с ножницами
        internal bool HitScissors(Point scissors)
        {
            if (scissors == null) return false;
            Point nextHead = GetNextPointPosition();
            return nextHead.IsHit(scissors);
        }

        //сокращение змейки
        internal void ShortenSnake()
        {
            int currentLength = pList.Count;
            if (currentLength <= MIN_SNAKE_LENGTH_AFTER_SCISSORS) return; // Не укорачивать, если змейка слишком короткая

            int segmentsToRemove = currentLength / 2; // Укоротить наполовину
            if (currentLength - segmentsToRemove < MIN_SNAKE_LENGTH_AFTER_SCISSORS)
            {
                // Корректировка, чтобы не сделать змейку короче минимальной длины
                segmentsToRemove = currentLength - MIN_SNAKE_LENGTH_AFTER_SCISSORS;
            }

            ConsoleColor originalBg = Console.BackgroundColor;
            Console.BackgroundColor = Program.bgColor; // Установка фона для корректной очистки
            for (int i = 0; i < segmentsToRemove && pList.Count > MIN_SNAKE_LENGTH_AFTER_SCISSORS; i++)
            {
                Point segment = pList[0]; // новый сегмент хвоста
                segment.Clear(); 
                pList.RemoveAt(0); //удал сегмент
            }
            Console.BackgroundColor = originalBg; // Восстановление фона
        }

        // Столкнулась ли голова змейки с ее хвостом
        internal bool IsHitTail()
        {
            if (pList == null || pList.Count <= 1) return false; // Не может столкнуться, если слишком короткая
            var head = pList[pList.Count - 1]; //(последний элемент)
            for (int i = 0; i < pList.Count - 1; i++) // Проверка столкновения со всеми сегментами, кроме головы
            {
                if (head.IsHit(pList[i])) return true;
            }
            return false;
        }

        // Позиция головы
        private Point GetNextPointPosition()
        {
            Point currentHead = pList[pList.Count - 1]; // Текущая голова (последний элемент)
            Point nextPos = new Point(currentHead);
            nextPos.Move(1, direction); // Двигаем точку в текущем направлении
            return nextPos;
        }

        //Управление клавишами движения
        public void HandleKey(ConsoleKey key)
        {
            if (key == ConsoleKey.LeftArrow && direction != Direction.RIGHT) direction = Direction.LEFT;
            else if (key == ConsoleKey.RightArrow && direction != Direction.LEFT) direction = Direction.RIGHT;
            else if (key == ConsoleKey.UpArrow && direction != Direction.DOWN) direction = Direction.UP;
            else if (key == ConsoleKey.DownArrow && direction != Direction.UP) direction = Direction.DOWN;
        }
    }
}