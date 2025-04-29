using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс для представления вертикальной линии, наследуется от Figure
    class VerticalLine : Figure
    {
        // Конструктор для создания вертикальной линии
        public VerticalLine(int yUp, int yDown, int x, char sym)
        {
            pList = new List<Point>(); // Инициализация списка точек
            for (int y = yUp; y <= yDown; y++) // Цикл для создания точек линии
            {
                Point p = new Point(x, y, sym); // Создание точки
                pList.Add(p);                  // Добавление точки в список
            }
        }
        // Метод Draw наследуется от базового класса Figure и не переопределяется здесь
    }
}
