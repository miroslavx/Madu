using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    // Класс, представляющий стены игрового поля
    class Walls
    {
        // Список фигур (линий), из которых состоят стены
        List<Figure> wallList;

        // Конструктор класса, создающий рамку по периметру карты
        public Walls(int mapWidth, int mapHeight)
        {
            wallList = new List<Figure>(); // Инициализация списка стен

            // Создание горизонтальных и вертикальных линий для рамки
            HorizontalLine upLine = new HorizontalLine(0, mapWidth - 2, 0, '+');
            HorizontalLine downLine = new HorizontalLine(0, mapWidth - 2, mapHeight - 1, '+');
            VerticalLine leftLine = new VerticalLine(0, mapHeight - 1, 0, '+');
            VerticalLine rightLine = new VerticalLine(0, mapHeight - 1, mapWidth - 2, '+');

            // Добавление созданных линий в список стен
            wallList.Add(upLine);
            wallList.Add(downLine);
            wallList.Add(leftLine);
            wallList.Add(rightLine);
        }

        // Метод для проверки, столкнулась ли переданная фигура со стенами
        internal bool IsHit(Figure figure)
        {
            foreach (var wall in wallList) // Перебираем каждую стену
            {
                if (wall.IsHit(figure)) // Проверяем пересечение фигуры со стеной
                {
                    return true; // Если есть пересечение хотя бы с одной стеной, возвращаем true
                }
            }
            return false; // Если пересечений нет
        }

        // Метод для отрисовки всех стен
        public void Draw()
        {
            foreach (var wall in wallList) // Перебираем и отрисовываем каждую стену
            {
                wall.Draw();
            }
        }
    }
}
