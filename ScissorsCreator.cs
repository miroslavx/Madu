using System;
using System.Collections.Generic;

namespace Snake
{
    // Создание объектов "ножниц" на игровом поле.
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

        // Создает объект "ножниц" в случайном свободном месте на карте.
        public Point CreateScissors(List<Point> snakeBody, Point foodPos, List<Figure> obstacles)
        {
            int x, y;
            bool collision;
            Point newScissorsLocation;
            do // Цикл для гарантии, что ножницы не появятся на занятом месте
            {
                // Генерация случайных координат
                x = random.Next(1, mapWidth - 1);
                y = random.Next(1, mapHeight - 1);
                newScissorsLocation = new Point(x, y, sym);
                collision = false;

                // Проверка на столкновение с телом змейки
                if (snakeBody != null)
                {
                    foreach (Point p in snakeBody)
                    {
                        if (p.IsHit(newScissorsLocation)) { collision = true; break; }
                    }
                }
                // Проверка на столкновение с едой (если она есть)
                if (!collision && foodPos != null && foodPos.IsHit(newScissorsLocation))
                {
                    collision = true;
                }
                // Проверка на столкновение с другими препятствиями
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
