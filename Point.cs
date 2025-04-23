// Point.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    // Класс Point определяет новый тип данных для точки на экране
    class Point
    {
        // Поля класса (данные, которые хранит точка)
        public int x;   
        public int y; 
        public char sym; 

        // Конструктор по умолчанию (без параметров)Вызывается, если создать объект так: new Point() в C# он создается неявно, если нет других конструкторов.
        public Point()
        {
        }


        public Point(int _x, int _y, char _sym)
        {
            x = _x;   
            y = _y;   
            sym = _sym;
        }

        // Метод класса для отрисовки точки
        public void Draw()
        {
            Console.SetCursorPosition(x, y); 
            Console.Write(sym);            // Выводим символ точки
        }
    }
}
