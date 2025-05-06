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

            permamentWallList.Add(new HorizontalLine(0, mapWidth - 1, 0, '+'));
            permamentWallList.Add(new HorizontalLine(0, mapWidth - 1, mapHeight - 1, '+'));
            permamentWallList.Add(new VerticalLine(1, mapHeight - 2, 0, '+'));
            permamentWallList.Add(new VerticalLine(1, mapHeight - 2, mapWidth - 1, '+'));
        }

        public void AddObstacle(Figure obstacle)
        {
            dynamicObstacles.Add(obstacle);
        }

        public void ClearDynamicObstacles()
        {
            ConsoleColor originalBg = Console.BackgroundColor;
            Console.BackgroundColor = Program.bgColor; // Устанавливаем фон для корректного стирания
            foreach (var obs in dynamicObstacles)
            {
                obs.Clear();
            }
            dynamicObstacles.Clear();
            Console.BackgroundColor = originalBg; // Восстанавливаем, если нужно
        }
        
        // Возвращает список текущих динамических препятствий (для проверки при создании еды/ножниц)
        public List<Figure> GetDynamicObstacles()
        {
            return dynamicObstacles;
        }


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

        // Отрисовка стен. Цвет устанавливается перед вызовом в Program.cs
        public void DrawPermanentWalls()
        {
            foreach (var wall in permamentWallList)
            {
                wall.Draw();
            }
        }
        public void DrawDynamicObstacles()
        {
            foreach (var obs in dynamicObstacles)
            {
                obs.Draw();
            }
        }
    }
}
