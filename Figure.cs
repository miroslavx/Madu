using System;
using System.Collections.Generic;

namespace Snake
{class Figure
    {
        // Список точек, из которых состоит фигура
        protected List<Point> pList;

        // Виртуальный метод для отрисовки фигуры (может быть переопределен в наследниках)
        public virtual void Draw()
        {
            foreach (Point p in pList)
            {
                p.Draw();
            }
        }

        // Метод для проверки столкновения текущей фигуры с другой фигурой
        internal bool IsHit(Figure figure)
        {
            foreach (var p in pList) // Перебираем все точки текущей фигуры
            {
                if (figure.IsHit(p)) // Проверяем, пересекается ли другая фигура с этой точкой
                {
                    return true; // Если хоть одна точка пересекается, значит фигуры столкнулись
                }
            }
            return false; // Если ни одна точка не пересеклась, столкновения нет
        }

        // Приватный вспомогательный метод для проверки столкновения фигуры с отдельной точкой
        private bool IsHit(Point point)
        {
            foreach (var p in pList) // Перебираем все точки текущей фигуры
            {
                if (p.IsHit(point)) // Сравниваем каждую точку фигуры с переданной точкой
                {
                    return true; // Если есть совпадение, значит точка попала на фигуру
                }
            }
            return false; // Если совпадений нет
        }
    }
}
