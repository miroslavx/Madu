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
        public Point CreateFood()
        {
            // Генерируем координаты, отступая от краев карты (индекс 2, чтобы не появляться на стенах)
            int x = random.Next(2, mapWidht - 2);
            int y = random.Next(2, mapHeight - 2);
            return new Point(x, y, sym); // Возвращаем новую точку
        }
    }
}
