using System;
using System.Collections.Generic;

namespace Snake
{
    // стена-препятствие
    class ObstacleWall : Figure
    {
        // Конструктор создает стену (горизонтальную или вертикальную) заданной длины
        // x, y - начальная точка, length - длина,isHorizontal - ориентация
        public ObstacleWall(int x, int y, int length, bool isHorizontal, char sym)
        {
            pList = new List<Point>();
            for (int i = 0; i < length; i++)
            {
                if (isHorizontal)
                {
                    pList.Add(new Point(x + i, y, sym));
                }
                else
                {
                    pList.Add(new Point(x, y + i, sym));
                }
            }
        }
    }
}