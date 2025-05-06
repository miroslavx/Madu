using System;
using System.Collections.Generic;
using System.Linq;

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

        internal void Move()
        {
            if (pList == null || !pList.Any()) return;
            
            Point tail = new Point(pList.First());
            Console.BackgroundColor = Program.bgColor;
            tail.Clear();

            pList.RemoveAt(0);
            Point head = GetNextPointPosition();
            pList.Add(head);
            
            // Цвет для головы устанавливается в Program.cs перед вызовом Draw()
            head.Draw();
        }

        internal bool Eat(Point food)
        {
            Point nextHead = GetNextPointPosition();
            if (nextHead.IsHit(food))
            {
                food.sym = Program.SNAKE_SYMBOL_CONST; 
                pList.Add(food);
                // Цвет для "новой головы" (съеденной еды) устанавливается в Program.cs перед Draw
                food.Draw();
                return true;
            }
            return false;
        }

        internal bool HitScissors(Point scissors)
        {
            if (scissors == null) return false;
            Point nextHead = GetNextPointPosition();
            return nextHead.IsHit(scissors);
        }

        internal void ShortenSnake()
        {
            int currentLength = pList.Count;
            if (currentLength <= MIN_SNAKE_LENGTH_AFTER_SCISSORS) return;
            int segmentsToRemove = currentLength / 2;
            if (currentLength - segmentsToRemove < MIN_SNAKE_LENGTH_AFTER_SCISSORS)
            {
                segmentsToRemove = currentLength - MIN_SNAKE_LENGTH_AFTER_SCISSORS;
            }
            Console.BackgroundColor = Program.bgColor;
            for (int i = 0; i < segmentsToRemove && pList.Count > MIN_SNAKE_LENGTH_AFTER_SCISSORS; i++)
            {
                Point segment = pList.First();
                segment.Clear();
                pList.RemoveAt(0);
            }
        }

        internal bool IsHitTail()
        {
            if (pList == null || pList.Count <= 1) return false;
            var head = pList.Last();
            for (int i = 0; i < pList.Count - 1; i++)
            {
                if (head.IsHit(pList[i])) return true;
            }
            return false;
        }
        
        private Point GetNextPointPosition()
        {
            Point currentHead = pList.Last();
            Point nextPos = new Point(currentHead);
            nextPos.Move(1, direction);
            return nextPos;
        }

        public void HandleKey(ConsoleKey key)
        {
            if (key == ConsoleKey.LeftArrow && direction != Direction.RIGHT) direction = Direction.LEFT;
            else if (key == ConsoleKey.RightArrow && direction != Direction.LEFT) direction = Direction.RIGHT;
            else if (key == ConsoleKey.UpArrow && direction != Direction.DOWN) direction = Direction.UP;
            else if (key == ConsoleKey.DownArrow && direction != Direction.UP) direction = Direction.DOWN;
        }
        // Метод Draw() из Figure используется. Цвет устанавливается в Program.cs.
    }
}
