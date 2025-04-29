using System;

namespace Snake
{
    // Класс для представления точки на консоли
    class Point
    {
        // Координата X точки
        public int x;
        // Координата Y точки
        public int y;
        // Символ для отображения точки
        public char sym;

        // Конструктор по умолчанию
        public Point()
        {
        }

        // Конструктор для создания точки с заданными координатами и символом
        public Point(int _x, int _y, char _sym)
        {
            x = _x;
            y = _y;
            sym = _sym;
        }

        // Конструктор копирования для создания точки на основе другой точки
        public Point(Point p)
        {
            x = p.x;
            y = p.y;
            sym = p.sym;
        }

        // Метод для смещения точки на заданное расстояние в указанном направлении
        public void Move(int offset, Direction direction)
        {
            if (direction == Direction.RIGHT)
            {
                x = x + offset;
            }
            else if (direction == Direction.LEFT)
            {
                x = x - offset;
            }
            else if (direction == Direction.UP)
            {
                y = y - offset; // В консоли ось Y направлена вниз
            }
            else if (direction == Direction.DOWN)
            {
                y = y + offset;
            }
        }

        // Метод для проверки, совпадают ли координаты этой точки с другой точкой
        public bool IsHit(Point p)
        {
            return p.x == this.x && p.y == this.y;
        }

        // Метод для отрисовки точки на консоли
        public void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.Write(sym);
        }

        // Метод для стирания точки с консоли (заменяет символ пробелом)
        public void Clear()
        {
            sym = ' ';
            Draw();
        }

        // Переопределение метода ToString для удобного вывода информации о точке (например, для отладки)
        public override string ToString()
        {
            return x + ", " + y + ", " + sym;
        }
    }
}
