using System;
using System.Collections.Generic;
namespace Snake
{
    // Класс для представления вертикальной линии, наследуется от Figure
    class VerticalLine : Figure
    {
        // Конструктор создает вертикальную линию
        public VerticalLine(int yUp, int yDown, int x, char sym)
        {
            pList = new List<Point>();
            for (int y = yUp; y <= yDown; y++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
        // Метод Draw() не переопределен, используется реализация из базового класса Figure
    }
}
