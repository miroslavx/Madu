using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    // Класс, представляющий змейку, наследуется от Figure
    class Snake : Figure
    {
        // Текущее направление движения змейки
        Direction direction;

        // Конструктор змейки
        public Snake(Point tail, int length, Direction _direction)
        {
            direction = _direction; // Устанавливаем начальное направление
            pList = new List<Point>(); // Инициализируем список точек змейки
            for (int i = 0; i < length; i++) // Создаем змейку заданной длины
            {
                Point p = new Point(tail); // Каждая новая точка начинается с позиции хвоста
                p.Move(i, direction);     // Смещаем точку для формирования тела змейки
                pList.Add(p);             // Добавляем точку в список
            }
        }

        // Метод для движения змейки на один шаг
        internal void Move()
        {
            Point tail = pList.First(); // Получаем точку хвоста (первая в списке)
            pList.Remove(tail);        // Удаляем хвост из списка
            Point head = GetNextPoint(); // Вычисляем новую позицию головы
            pList.Add(head);           // Добавляем новую голову в конец списка

            tail.Clear();             // Стираем старый хвост с консоли
            head.Draw();              // Рисуем новую голову на консоли
        }

        // Метод для проверки, съела ли змейка еду
        internal bool Eat(Point food)
        {
            Point head = GetNextPoint(); // Получаем предполагаемую следующую позицию головы
            if (head.IsHit(food)) // Проверяем, совпадает ли она с позицией еды
            {
                food.sym = head.sym; // Меняем символ еды на символ головы змейки
                pList.Add(food);    // Добавляем точку (бывшую еду) к телу змейки
                return true;       // Возвращаем true, так как еда съедена
            }
            else
            {
                return false;      // Еда не съедена
            }
        }

        // Проверка столкновения головы с хвостом
        internal bool IsHitTail()
        {
            var head = pList.Last(); // Получаем текущую голову
            for(int i = 0; i < pList.Count - 2; i++) // Проверяем все точки, кроме последних двух (головы и предголовы)
            {
                if(head.IsHit( pList[ i ] ) ) // Если голова столкнулась с точкой хвоста
                    return true;
            }
            return false; // Если столкновений нет
        }

        //  метод для вычисления следующей позиции головы
        private Point GetNextPoint()
        {
            Point head = pList.Last();      // Текущая голова - последняя точка в списке
            Point nextPoint = new Point(head); // Создаем копию головы
            nextPoint.Move(1, direction);   // Сдвигаем копию на один шаг в текущем направлении
            return nextPoint;              // Возвращаем новую позицию
        }

        // Метод для обработки нажатий клавиш управления
        public void HandleKey(ConsoleKey key)
        {
            // Меняем направление движения змейки в зависимости от нажатой стрелки
            if (key == ConsoleKey.LeftArrow)
                direction = Direction.LEFT;
            else if (key == ConsoleKey.RightArrow)
                direction = Direction.RIGHT;
            else if (key == ConsoleKey.DownArrow)
                direction = Direction.DOWN;
            else if (key == ConsoleKey.UpArrow)
                direction = Direction.UP;
        }
    }
}
