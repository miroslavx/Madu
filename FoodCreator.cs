using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    // Класс, отвечающий за создание еды на игровом поле
    class FoodCreator
    {
        // Ширина игрового поля
        int mapWidht;
        // Высота игрового поля
        int mapHeight;
        // Символ, которым отображается еда
        char sym;

        // Генератор случайных чисел для определения координат еды
        Random random = new Random();

        // Конструктор класса, принимающий размеры карты и символ еды
        public FoodCreator(int mapWidth, int mapHeight, char sym)
        {
            this.mapWidht = mapWidth;   // Сохраняем ширину
            this.mapHeight = mapHeight; // Сохраняем высоту
            this.sym = sym;             // Сохраняем символ
        }

        // Метод для создания точки еды в случайном месте карты (внутри границ)
        public Point CreateFood()
        {
            // Генерируем случайную X координату (с отступом от краев)
            int x = random.Next(2, mapWidht - 2);
            // Генерируем случайную Y координату (с отступом от краев)
            int y = random.Next(2, mapHeight - 2);
            // Возвращаем новую точку с вычисленными координатами и заданным символом
            return new Point(x, y, sym);
        }
    }
}
