// Snake.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    // Класс, представляющий змейку, наследуется от Figure
    class Snake : Figure
    {
        Direction direction; // Текущее направление движения змейки
        private Point tail; // Храним хвост для очистки
        private Point head; // Храним голову

        // Минимальная длина змейки после укорачивания
        const int MIN_LENGTH = 3;

        // Конструктор создает змейку из 'length' сегментов, начиная от 'tailPos', движущуюся в 'direction'
        public Snake(Point tailPos, int length, Direction _direction)
        {
            direction = _direction;
            pList = new List<Point>();
            for (int i = 0; i < length; i++)
            {
                // *** ИЗМЕНЕНИЕ: Убедимся что стартовый символ правильный ***
                // Символ берется из tailPos, который должен быть '*' в Program.cs
                Point p = new Point(tailPos);
                p.Move(i, direction);
                pList.Add(p);
            }
            head = pList.Last(); // Последняя добавленная точка - голова
            tail = pList.First(); // Первая - хвост
        }

        // Метод для движения змейки на один шаг
        internal void Move()
        {
            tail = pList.First();      // Определяем текущий хвост
            pList.Remove(tail);         // Удаляем хвост из списка (но пока не стираем)
            head = GetNextPoint();      // Вычисляем новую позицию головы
            pList.Add(head);            // Добавляем новую голову в конец списка

            tail.Clear();               // Стираем старый хвост с экрана
            head.Draw();                // Рисуем новую голову (с текущими цветами консоли и символом '*')
        }

        // Метод для проверки, съела ли змейка еду 'food'
        internal bool Eat(Point food)
        {
            Point nextHeadCandidate = GetNextPoint(); // Получаем предполагаемую новую позицию головы
            if (nextHeadCandidate.IsHit(food))      // Если голова оказывается на месте еды
            {
                // *** ИЗМЕНЕНИЕ: Символ новой головы - это символ текущей головы (*) ***
                food.sym = pList.Last().sym; // Новый сегмент получает символ змейки
                pList.Add(food);            // Добавляем съеденную еду как новый сегмент змейки
                head = food;                // Голова теперь это съеденная точка еды
                // Не удаляем хвост в этом ходе, змейка растет
                head.Draw();                // Рисуем новую голову (съеденную еду как часть змейки)
                return true;                // Возвращаем true (еда съедена)
            }
            else
            {
                return false;               // Возвращаем false (еда не съедена)
            }
        }

        // Проверка столкновения с ножницами
        internal bool HitScissors(Point scissors)
        {
            Point nextHeadCandidate = GetNextPoint(); // Получаем предполагаемую новую позицию головы
                                                      // Проверяем столкновение будущей головы с ножницами
                                                      // Добавим проверку scissors на null на всякий случай
            return scissors != null && nextHeadCandidate.IsHit(scissors);
        }

        // Укоротить змейку
        internal void ShortenSnake()
        {
            int currentLength = pList.Count;
            if (currentLength <= MIN_LENGTH) return; // Не укорачиваем, если и так короткая

            int segmentsToRemove = currentLength / 2; // Удаляем половину
            // Если удаление половины сделает змейку слишком короткой,
            // удаляем столько, чтобы осталась минимальная длина
            if (currentLength - segmentsToRemove < MIN_LENGTH)
            {
                segmentsToRemove = currentLength - MIN_LENGTH;
            }

            // Удаляем сегменты с хвоста
            for (int i = 0; i < segmentsToRemove; i++)
            {
                if (pList.Any()) // Проверка, что список не пуст
                {
                    Point segmentToRemove = pList.First(); // Берем сегмент хвоста
                    segmentToRemove.Clear();            // Стираем его с экрана
                    pList.RemoveAt(0);                  // Удаляем из списка
                }
                else
                {
                    break; // Выходим, если список неожиданно опустел
                }
            }
            // Обновляем указатель на хвост после удаления
            tail = pList.FirstOrDefault(); // Берем новый первый элемент или null, если список пуст
        }


        // Метод для проверки столкновения головы змейки со своим хвостом
        internal bool IsHitTail()
        {
            var nextHead = GetNextPoint(); // Будущая позиция головы
            // Перебираем все точки тела, КРОМЕ текущей головы (последний элемент)
            for (int i = 0; i < pList.Count - 1; i++)
            {
                if (nextHead.IsHit(pList[i])) // Если будущая голова столкнется с точкой хвоста
                    return true;
            }
            return false;
        }


        // Приватный метод для вычисления следующей позиции головы
        private Point GetNextPoint()
        {
            // Голова - это всегда последний элемент списка
            Point currentHead = pList.LastOrDefault();
            // Если список пуст (не должно быть, но для безопасности)
            if (currentHead == null) throw new InvalidOperationException("Ussil pole pead!");

            Point nextPoint = new Point(currentHead); // Создаем копию текущей головы (с ее символом '*')
            nextPoint.Move(1, direction);     // Смещаем копию на 1 шаг в текущем направлении
            return nextPoint;                 // Возвращаем новую позицию
        }

        // Метод для обработки нажатия клавиш и изменения направления змейки
        public void HandleKey(ConsoleKey key)
        {
            // Предотвращаем разворот на 180 градусов
            if (key == ConsoleKey.LeftArrow && direction != Direction.RIGHT)
                direction = Direction.LEFT;
            else if (key == ConsoleKey.RightArrow && direction != Direction.LEFT)
                direction = Direction.RIGHT;
            else if (key == ConsoleKey.DownArrow && direction != Direction.UP)
                direction = Direction.DOWN;
            else if (key == ConsoleKey.UpArrow && direction != Direction.DOWN)
                direction = Direction.UP;
        }

        // --- Убрана логика отрисовки головы отдельным символом ---
        // Используем базовый метод Draw из Figure, который просто рисует все точки
        // public override void Draw() { ... } // Можно удалить, используется базовый

        // Используем базовый метод Draw(colors) из Figure
        // public override void Draw(ConsoleColor foreground, ConsoleColor background) { ... } // Можно удалить
    }
}