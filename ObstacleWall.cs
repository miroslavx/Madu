using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс ObstacleWall: Представляет стену-препятствие
    class ObstacleWall : Figure
    {
        // Конструктор создает стену (горизонтальную или вертикальную) заданной длины x, y - начальная точка, length - длина, isHorizontal - ориентация (true - горизонтальная, false - вертикальная).
        public ObstacleWall(int x, int y, int length, bool isHorizontal, char sym)
        {
            pList = new List<Point>();
            // Создание точек, составляющих стену
            for (int i = 0; i < length; i++)
            {
                if (isHorizontal)
                {
                    // Горизонтальная стена
                    pList.Add(new Point(x + i, y, sym));
                }
                else
                {
                    // Вертикальная стена
                    pList.Add(new Point(x, y + i, sym));
                }
            }
        }
    }
}