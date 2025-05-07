using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс ScissorsCreator: Отвечает за создание объектов "ножниц" на игровом поле.
    class ScissorsCreator
    {
        int mapWidth;
        int mapHeight;
        char sym;
        Random random = new Random();

        public ScissorsCreator(int width, int height, char scissorsSymbol)
        {
            mapWidth = width;
            mapHeight = height;
            sym = scissorsSymbol;
        }

        public Point CreateScissors(List<Point> snakeBody, Point foodPos, List<Figure> obstacles)
        {
            int x, y;
            bool collision;
            Point newScissorsLocation;
            do
            {
                x = random.Next(1, mapWidth - 1);
                y = random.Next(1, mapHeight - 1);
                newScissorsLocation = new Point(x, y, sym);
                collision = false;

                if (snakeBody != null)
                {
                    foreach (Point p in snakeBody)
                    {
                        if (p.IsHit(newScissorsLocation)) { collision = true; break; }
                    }
                }
                if (!collision && foodPos != null && foodPos.IsHit(newScissorsLocation))
                {
                    collision = true;
                }
                if (!collision && obstacles != null)
                {
                    foreach (Figure obs in obstacles)
                    {
                        if (obs.ContainsPoint(newScissorsLocation))
                        {
                            collision = true;
                            break;
                        }
                    }
                }
            } while (collision);
            return newScissorsLocation;
        }
    }
}
