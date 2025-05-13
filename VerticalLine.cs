using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс VerticalLine: Представляет вертикальную линию, наследуется от Figure.
    class VerticalLine : Figure
    {
        // Создает вертикальную линию из точек
        public VerticalLine(int yUp, int yDown, int x, char sym)
        {
            pList = new List<Point>();
            for (int y = yUp; y <= yDown; y++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }
}
