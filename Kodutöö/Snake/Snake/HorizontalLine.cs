// HorizontalLine.cs
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

        // Метод Draw() теперь не переопределен,
        // будет использоваться реализация из Figure,
        // которая просто рисует точки текущими цветами.
        // public override void Draw()
        // {
        //     Console.ForegroundColor = ConsoleColor.Yellow; // Убираем это
        //     base.Draw();
        //     Console.ForegroundColor = ConsoleColor.White; // Убираем это
        // }
    }
}