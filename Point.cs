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

        public Point(Point p) // Конструктор копирования
        {
            x = p.x;
            y = p.y;
            sym = p.sym;
        }

        // Сдвигает точку на указанное смещение в заданном направлении.
        public void Move(int offset, Direction direction)
        {
            if (direction == Direction.RIGHT) x += offset;
            else if (direction == Direction.LEFT) x -= offset;
            else if (direction == Direction.UP) y -= offset;
            else if (direction == Direction.DOWN) y += offset;
        }

        // Проверяет, совпадают ли координаты этой точки с другой точкой.
        public bool IsHit(Point p)
        {
            if (p == null) return false;
            return p.x == this.x && p.y == this.y;
        }

        // Отрисовывает символ точки в консоли.
        public void Draw()
        {
            // Проверка, чтобы не пытаться рисовать за пределами буфера консоли
            if (x >= 0 && x < Console.BufferWidth && y >= 0 && y < Console.BufferHeight)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(sym);
            }
        }

        // Очищает точку с экрана (рисует пробел).
        public void Clear()
        {
            sym = ' '; // Заменяем символ на пробел
            Draw();    // Отрисовываем пробел на месте точки
        }

        public override string ToString()
        {
            return x + ", " + y + ", " + sym;
        }
    }
}
