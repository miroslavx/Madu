using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс Figure: Базовый класс для всех игровых фигур, состоящих из списка точек предоставляет общие методы для отрисовки и проверки столкновений.
    class Figure
    {
        protected List<Point> pList;

        public virtual void Draw()
        {
            if (pList == null) return;
            foreach (Point p in pList)
            {
                p.Draw(); // Отрисовка каждой точки фигуры с текущим цветом консоли
            }
        }

        public virtual void Clear()
        {
            if (pList == null) return;
            foreach (Point p in pList)
            {
                p.Clear();
            }
        }

        public List<Point> GetPoints()
        {
            return pList;
        }

        internal bool IsHit(Figure figure)
        {
            if (pList == null || figure == null || figure.GetPoints() == null) return false;
            foreach (var p_current in pList)
            {
                if (figure.ContainsPoint(p_current))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool ContainsPoint(Point point)
        {
            if (pList == null || point == null) return false;
            foreach (var p_figure in pList)
            {
                if (p_figure.IsHit(point))
                {
                    return true;
                }
            }
            return false;
        }
    }
}