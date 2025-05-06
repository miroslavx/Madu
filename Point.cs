using System;

namespace Snake
{
    // Класс Point: Представляет точку с координатами (x, y) и символом для отображения на консоли.
    class Point
    {
        public int x;
        public int y;
        public char sym;

        public Point() { }
        public Point(int _x, int _y, char _sym)
        {
            x = _x;
            y = _y;
            sym = _sym;
        }

        public Point(Point p)
        {
            x = p.x;
            y = p.y;
            sym = p.sym;
        }

        public void Move(int offset, Direction direction)
        {
            if (direction == Direction.RIGHT) x += offset;
            else if (direction == Direction.LEFT) x -= offset;
            else if (direction == Direction.UP) y -= offset;
            else if (direction == Direction.DOWN) y += offset;
        }

        public bool IsHit(Point p)
        {
            if (p == null) return false;
            return p.x == this.x && p.y == this.y;
        }

        public void Draw()
        {
            if (x >= 0 && x < Console.BufferWidth && y >= 0 && y < Console.BufferHeight)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(sym);
            }
        }

        public void Clear()
        {
            sym = ' ';
            Draw();
        }

        public override string ToString()
        {
            return x + ", " + y + ", " + sym;
        }
    }
}
