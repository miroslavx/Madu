using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс Walls: Управляет стенами игрового поля, включая постоянную рамку и динамические препятствия.
    class Walls
    {
        List<Figure> permamentWallList;
        List<Figure> dynamicObstacles;

        public Walls(int mapWidth, int mapHeight)
        {
            permamentWallList = new List<Figure>();
            dynamicObstacles = new List<Figure>();

            // Инициализация постоянных стен (рамки игрового поля)
            permamentWallList.Add(new HorizontalLine(0, mapWidth - 1, 0, '+'));
            permamentWallList.Add(new HorizontalLine(0, mapWidth - 1, mapHeight - 1, '+'));
            permamentWallList.Add(new VerticalLine(1, mapHeight - 2, 0, '+'));
            permamentWallList.Add(new VerticalLine(1, mapHeight - 2, mapWidth - 1, '+'));
        }

        public void AddObstacle(Figure obstacle)
        {
            dynamicObstacles.Add(obstacle);
        }

        // Очищает динамические препятствия с экрана и из списка
        public void ClearDynamicObstacles()
        {
            ConsoleColor originalBg = Console.BackgroundColor;
            Console.BackgroundColor = Program.bgColor; // Устанавливаем фон для корректного стирания
            foreach (var obs in dynamicObstacles)
            {
                obs.Clear();
            }
            dynamicObstacles.Clear();
            Console.BackgroundColor = originalBg; // Восстанавливаем, если нужно было менять локально
        }

        public List<Figure> GetDynamicObstacles()
        {
            return dynamicObstacles;
        }

        // Проверяет, столкнулась ли фигура (например, змейка) со стеной
        internal bool IsHit(Figure figure)
        {
            foreach (var wall in permamentWallList)
            {
                if (wall.IsHit(figure)) return true;
            }
            foreach (var obs in dynamicObstacles)
            {
                if (obs.IsHit(figure)) return true;
            }
            return false;
        }
        // Отрисовывает постоянные стены
        public void DrawPermanentWalls()
        {
            foreach (var wall in permamentWallList)
            {
                wall.Draw();
            }
        }
        // Отрисовывает динамические препятствия
        public void DrawDynamicObstacles()
        {
            foreach (var obs in dynamicObstacles)
            {
                obs.Draw();
            }
        }
    }
}
