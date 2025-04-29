using System;
using System.Collections.Generic;

namespace Snake
{
    // Класс для представления горизонтальной линии, наследуется от Figure
    class HorizontalLine : Figure
    {
        // Конструктор создает горизонтальную линию
        public HorizontalLine(int xLeft, int xRight, int y, char sym)
        {
            pList = new List<Point>();
            for (int x = xLeft; x <= xRight; x++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
        // Переопределенный метод отрисовки для горизонтальной линии
        // Рисует линию желтым цветом
        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            // Вызов базовой реализации Draw() из класса Figure (или своя логика отрисовки)
            base.Draw(); // Используем базовую отрисовку точек
            Console.ForegroundColor = ConsoleColor.White; // Возвращаем цвет по умолчанию
        }
    }
}
