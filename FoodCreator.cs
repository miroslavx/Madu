using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс FoodCreator: Отвечает за создание объектов еды на игровом поле.
    class FoodCreator
    {
        int mapWidth;
        int mapHeight;
        char sym;
        Random random = new Random();

        public FoodCreator(int width, int height, char foodSymbol)
        {
            mapWidth = width;
            mapHeight = height;
            sym = foodSymbol;
        }

        // Создает объект еды в случайном свободном месте на карте.
        public Point CreateFood(List<Point> snakeBody, Point currentScissors, List<Figure> obstacles)
        {
            int x, y;
            bool collision;
            Point newFoodLocation;
            do // для гарантии, что еда не появится на занятом месте
            {
                // Генерация случайных координат в пределах игрового поля (исключая границы)
                x = random.Next(1, mapWidth - 1);
                y = random.Next(1, mapHeight - 1);
                newFoodLocation = new Point(x, y, sym);
                collision = false;

                // Проверка на столкновение с телом змейки
                if (snakeBody != null)
                {
                    foreach (Point p in snakeBody)
                    {
                        if (p.IsHit(newFoodLocation)) { collision = true; break; }
                    }
                }
                // Проверка на столкновение с ножницами
                if (!collision && currentScissors != null && currentScissors.IsHit(newFoodLocation))
                {
                    collision = true;
                }
                // Проверка на столкновение с другими препятствиями
                if (!collision && obstacles != null)
                {
                    foreach (Figure obs in obstacles)
                    {
                        if (obs.ContainsPoint(newFoodLocation)) 
                        {
                            collision = true;
                            break;
                        }
                    }
                }
            } while (collision);
            return newFoodLocation;
        }
    }
}
