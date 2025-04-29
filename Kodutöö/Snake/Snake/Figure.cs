// Figure.cs
using System;
using System.Collections.Generic;

namespace Snake
{
    // Базовый класс для всех фигур в игре (линии, змейка)
    class Figure
    {
        // Список точек, из которых состоит фигура
        // *** ИЗМЕНЕНИЕ: protected -> internal ***
        internal List<Point> pList; // Делаем доступным внутри проекта

        // Стандартный метод отрисовки (использует текущие цвета консоли)
        public virtual void Draw()
        {
            foreach (Point p in pList)
            {
                p.Draw();
            }
        }

        // Новый метод отрисовки с указанием цветов
        public virtual void Draw(ConsoleColor foreground, ConsoleColor background)
        {
            // Устанавливаем цвета перед отрисовкой всей фигуры
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            foreach (Point p in pList)
            {
                p.Draw(); // Точка рисуется с установленными цветами
            }
            // Не сбрасываем цвета здесь, чтобы вызывающий код мог управлять ими
        }

        // Проверяет, пересекается ли текущая фигура с другой фигурой 'figure'
        internal bool IsHit(Figure figure)
        {
            foreach (var p in pList) // Для каждой точки текущей фигуры
            {
                if (figure.IsHit(p)) // Проверяем, пересекается ли она с другой фигурой
                {
                    return true; // Если хоть одна точка пересеклась, возвращаем true
                }
            }
            return false; // Если ни одна точка не пересеклась
        }

        // Вспомогательный метод: проверяет, пересекается ли фигура с точкой 'point'
        // Сделали internal ранее, это правильно
        internal bool IsHit(Point point)
        {
            foreach (var p in pList) // Для каждой точки текущей фигуры
            {
                if (p.IsHit(point)) // Сравниваем ее с переданной точкой
                {
                    return true;
                }
            }
            return false;
        }
    }
}