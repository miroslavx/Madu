using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс для представления горизонтальной линии, наследуется от Figure
    class HorizontalLine : Figure
    {
        // Конструктор для создания горизонтальной линии
        public HorizontalLine(int xLeft, int xRight, int y, char sym)
        {
            pList = new List<Point>(); // Инициализация списка точек
            for (int x = xLeft; x <= xRight; x++) // Цикл для создания точек линии
            {
                Point p = new Point(x, y, sym); // Создание точки
                pList.Add(p);                  // Добавление точки в список
            }
        }

        // Переопределенный метод отрисовки для горизонтальной линии (рисует желтым цветом)
        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Yellow; // Устанавливаем желтый цвет
            foreach (Point p in pList) // Отрисовываем каждую точку линии
            {
                p.Draw();
            }
            // base.Draw(); // Можно вызвать базовую реализацию, если нужно (здесь не используется)
            Console.ForegroundColor = ConsoleColor.White; // Возвращаем цвет консоли по умолчанию
        }
    }
}
