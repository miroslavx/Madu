// Walls.cs
using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс, представляющий стены игрового поля
    class Walls
    {
        List<Figure> wallList; // Список фигур (линий), из которых состоят стены

        // Конструктор создает стены по периметру карты заданных размеров
        public Walls(int mapWidth, int mapHeight)
        {
            wallList = new List<Figure>();

            // Создание горизонтальных и вертикальных линий для рамки
            // Используем символ '+', он обычно хорошо виден на разных фонах
            HorizontalLine upLine = new HorizontalLine(0, mapWidth - 2, 0, '+');
            HorizontalLine downLine = new HorizontalLine(0, mapWidth - 2, mapHeight - 1, '+');
            VerticalLine leftLine = new VerticalLine(0, mapHeight - 1, 0, '+');
            VerticalLine rightLine = new VerticalLine(0, mapHeight - 1, mapWidth - 2, '+');

            // Добавление линий в список стен
            wallList.Add(upLine);
            wallList.Add(downLine);
            wallList.Add(leftLine);
            wallList.Add(rightLine);
        }

        // Проверяет, столкнулась ли переданная фигура 'figure' с какой-либо из стен
        internal bool IsHit(Figure figure)
        {
            foreach (var wall in wallList)
            {
                // Проверяем пересечение переданной фигуры со стеной (wall)
                // wall.IsHit(figure) вызовет Figure.IsHit(Figure),
                // который проверит каждую точку стены против каждой точки фигуры.
                if (wall.IsHit(figure))
                {
                    return true; // Столкновение обнаружено
                }
            }
            return false; // Столкновений нет
        }

        // --- Метод Draw БЕЗ параметров ---
        // Отрисовывает все стены на консоли, используя текущие установленные цвета консоли
        public void Draw()
        {
            foreach (var wall in wallList)
            {
                // Вызывает Figure.Draw() без параметров для каждой линии стены
                wall.Draw();
            }
        }

        // --- Метод Draw С параметрами ЦВЕТА ---
        // Отрисовывает все стены с заданными цветами
        public void Draw(ConsoleColor foreground, ConsoleColor background)
        {
            foreach (var wall in wallList)
            {
                // Вызывает Figure.Draw(foreground, background) для каждой линии стены,
                // передавая указанные цвета. Figure.Draw в свою очередь установит
                // эти цвета и отрисует все точки линии.
                wall.Draw(foreground, background);
            }
        }

    } // Конец класса Walls
} // Конец namespace Snake