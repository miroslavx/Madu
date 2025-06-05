using System;
using System.Collections.Generic;

namespace Snake
{
    // Для всех игровых фигур, состоящих из списка точек, предоставляет общие методы для отрисовки и проверки столкновений.
    class Figure
    {
        protected List<Point> pList; // Список точек, составляющих фигуру

        // Отрисовывает все точки фигуры.
        public virtual void Draw()
        {
            if (pList == null) return;
            foreach (Point p in pList)
            {
                p.Draw();
            }
        }

        // Очищает все точки фигуры с экрана.
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

        // Проверяет, пересекается ли эта фигура с другой фигурой.
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

        // Проверяет, содержит ли фигура указанную точку.
        internal bool ContainsPoint(Point point)
        {
            if (pList == null || point == null) return false;
            // Проверяем, совпадает ли какая-либо точка фигуры с указанной точкой
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