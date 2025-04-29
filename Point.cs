using System;
namespace Snake
{
    // Класс для представления точки на консоли
    class Point
    {
        public int x; // Координата X
        public int y; // Координата Y
        public char sym; // Символ для отображения точки
        // Пустой конструктор по умолчанию
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

        // Конструктор копирования: создает новую точку на основе существующей
        public Point(Point p)
        {
            x = p.x;
            y = p.y;
            sym = p.sym;
        }

        // Смещает точку на 'offset' единиц в указанном 'direction'
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
                y = y - offset; // В консоли Y уменьшается при движении вверх
            }
            else if (direction == Direction.DOWN)
            {
                y = y + offset; // В консоли Y увеличивается при движении вниз
            }
        }

        // Проверяет, совпадают ли координаты текущей точки с другой точкой 'p'
        public bool IsHit(Point p)
        {
            return p.x == this.x && p.y == this.y;
        }
        // Отображает точку на консоли
        public void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.Write(sym);
        }
        // Стирает точку с консоли (заменяет символ пробелом и перерисовывает)
        public void Clear()
        {
            sym = ' ';
            Draw();
        }
        // Возвращает строковое представление точки (для отладки)
        public override string ToString()
        {
            return x + ", " + y + ", " + sym;
        }
    }
}
