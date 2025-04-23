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
        public int x;   // Координата X точки
        public int y;   // Координата Y точки
        public char sym; // Символ для отображения точки

        // Метод (функция) класса для отрисовки точки Метод принадлежит объекту класса Point и работает с его полями (x, y, sym) не static, так как вызывается для конкретного объекта (p1.Draw(), p2.Draw())
        public void Draw()
        {
            Console.SetCursorPosition(x, y); // Устанавливаем курсор в координаты точки (x, y)
            Console.Write(sym);            // Выводим символ точки (sym)
        }
    }
}
