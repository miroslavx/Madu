using System;
using System.Collections.Generic;
namespace Snake
{
    // Базовый класс для всех фигур в игре (линии, змейка)
    class Figure
    {
        // Список точек, из которых состоит фигура
        protected List<Point> pList;

        // Виртуальный метод для отрисовки фигуры.
        // Может быть переопределен в классах-наследниках.
        public virtual void Draw()
        {
            foreach (Point p in pList)
            {
                p.Draw();
            }
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

        // Приватный вспомогательный метод: проверяет, пересекается ли фигура с точкой 'point'
        private bool IsHit(Point point)
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
