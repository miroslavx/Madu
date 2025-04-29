// FoodCreator.cs
using System;

namespace Snake
{
    // Класс, отвечающий за создание еды в случайном месте карты
    class FoodCreator
    {
        int mapWidht;  // Ширина карты
        int mapHeight; // Высота карты
        char sym;      // Символ для еды

        Random random = new Random(); // Генератор случайных чисел

        // Конструктор сохраняет размеры карты и символ еды
        public FoodCreator(int mapWidth, int mapHeight, char sym)
        {
            this.mapWidht = mapWidth;
            this.mapHeight = mapHeight;
            this.sym = sym;
        }

        // Создает и возвращает точку еды в случайных координатах в пределах карты
        // Добавляем параметры, чтобы еда не появлялась на змейке или ножницах
        public Point CreateFood(System.Collections.Generic.List<Point> snakeBody, Point scissorsLocation)
        {
            int x, y;
            bool collision;
            do
            {
                // Генерируем координаты, отступая от краев карты (индекс 2)
                x = random.Next(2, mapWidht - 2);
                y = random.Next(2, mapHeight - 2);

                // Проверяем, не совпали ли координаты с телом змейки или ножницами
                collision = false;
                foreach (Point p in snakeBody)
                {
                    if (p.x == x && p.y == y)
                    {
                        collision = true;
                        break;
                    }
                }
                if (!collision && scissorsLocation != null && scissorsLocation.x == x && scissorsLocation.y == y)
                {
                    collision = true;
                }

            } while (collision); // Повторяем, если координаты заняты

            return new Point(x, y, sym); // Возвращаем новую точку
        }
    }
}