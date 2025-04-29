// ScissorsCreator.cs
using System;

namespace Snake
{
    // Класс, отвечающий за создание ножниц в случайном месте карты
    class ScissorsCreator
    {
        int mapWidht;  // Ширина карты
        int mapHeight; // Высота карты
        char sym;      // Символ для ножниц

        Random random = new Random(); // Генератор случайных чисел

        // Конструктор сохраняет размеры карты и символ ножниц
        public ScissorsCreator(int mapWidth, int mapHeight, char sym)
        {
            this.mapWidht = mapWidth;
            this.mapHeight = mapHeight;
            this.sym = sym;
        }

        // Создает и возвращает точку ножниц в случайных координатах в пределах карты
        // Добавим параметр snakeBody, чтобы ножницы не появлялись на змейке
        public Point CreateScissors(System.Collections.Generic.List<Point> snakeBody, Point foodLocation)
        {
            int x, y;
            bool collision;
            do
            {
                // Генерируем координаты, отступая от краев карты (индекс 1 или 2)
                x = random.Next(2, mapWidht - 2);
                y = random.Next(2, mapHeight - 2);

                // Проверяем, не совпали ли координаты с телом змейки или едой
                collision = false;
                foreach (Point p in snakeBody)
                {
                    if (p.x == x && p.y == y)
                    {
                        collision = true;
                        break;
                    }
                }
                if (!collision && foodLocation != null && foodLocation.x == x && foodLocation.y == y)
                {
                    collision = true;
                }

            } while (collision); // Повторяем, если координаты заняты

            return new Point(x, y, sym); // Возвращаем новую точку
        }
    }
}