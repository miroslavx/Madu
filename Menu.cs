using System;
using System.Collections.Generic;
using System.IO;

namespace Snake
{
    // Класс Menu: Управляет отображением главного меню, таблицы рекордов и настройками цветовой схемы.
    static class Menu
    {
        public static void ShowMainMenu(out bool exitSelected)
        {
            exitSelected = false;
            Program.gameSounds?.PlayBackground();

            Console.BackgroundColor = Program.bgColor;
            Console.Clear();
            Program.SetCurrentColors(Program.fgColorText, Program.bgColor);

            int centerAlignX = (Console.BufferWidth / 2);

            Program.WriteTextAt("╔════════════════════════╗", centerAlignX - 14, 3);
            Program.WriteTextAt("║        S N A K E       ║", centerAlignX - 14, 4);
            Program.WriteTextAt("╚════════════════════════╝", centerAlignX - 14, 5);

            string[] menuItems = {
                "1. Alusta mängu (Klassikaline)",
                "2. Mäng ajale (Kiire)",
                "3. Rekordid",
                "4. Värvivalik",
                "5. Välju"
            };
            int menuY = 8;
            int menuItemWidth = 30;
            foreach (string item in menuItems)
            {
                Program.WriteTextAt($"   {item.PadRight(menuItemWidth - 6)}", centerAlignX - menuItemWidth / 2, menuY++);
            }
            Program.WriteTextAt("".PadRight(menuItemWidth, '-'), centerAlignX - menuItemWidth / 2, menuY++);
            Program.WriteTextAt("Vali number: ", Math.Max(0, centerAlignX - 12), menuY); // Используем menuY напрямую

            Console.CursorVisible = true;
            string choice = Console.ReadLine();
            Console.CursorVisible = false;

            switch (choice)
            {
                case "1":
                    Program.gameSounds?.StopBackground();
                    Game.PlayClassicGame();
                    break;
                case "2":
                    Program.gameSounds?.StopBackground();
                    TimeGame.PlayTimeAttackGame();
                    break;
                case "3":
                    ShowHighScores();
                    break;
                case "4":
                    SelectColorScheme();
                    break;
                case "5":
                    exitSelected = true;
                    Program.gameSounds?.StopBackground();
                    break;
                default:
                    Program.WriteTextAt("Vale valik. Vajuta Enter...", centerAlignX - 10, menuY + 2); 
                    Console.ReadLine();
                    break;
            }
        }

        public static void SaveScore(int score, string gameModeTag)
        {
            Console.CursorVisible = true;
            Program.SetCurrentColors(Program.fgColorText, Program.bgColor);

            // Определяем позицию для "Sisesta oma nimi"
            int promptY = Program.MAP_HEIGHT / 2 + 3;
            int promptX = (Console.BufferWidth / 2) - 20;
            Program.WriteTextAt("Sisesta oma nimi (min 3 tähte): ", promptX, promptY);

            string playerName = "";
            Console.SetCursorPosition(promptX + "Sisesta oma nimi (min 3 tähte): ".Length, promptY);
            while (true)
            {
                playerName = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(playerName) && playerName.Length >= 3) break;

                // Сообщение об ошибке и повторный ввод
                Program.SetCurrentColors(ConsoleColor.Red, Program.bgColor); // Выделим ошибку
                Program.WriteTextAt("Nimi peab olema vähemalt 3 tähte! Proovi uuesti: ".PadRight(Console.BufferWidth - (promptX + 1)), promptX, promptY + 1); // Очистка строки и сообщение
                Program.SetCurrentColors(Program.fgColorText, Program.bgColor);
                Console.SetCursorPosition(promptX + "Nimi peab olema vähemalt 3 tähte! Proovi uuesti: ".Length, promptY + 1); // Курсор для нового ввода
            }
            Console.CursorVisible = false;

            // Очистка строки с ошибкой, если она была
            Program.WriteTextAt(" ".PadRight(Console.BufferWidth - (promptX + 1)), promptX, promptY + 1);


            try
            {
                using (StreamWriter sw = new StreamWriter(Program.SCORES_FILENAME, true))
                {
                    sw.WriteLine($"[{gameModeTag}] {playerName}:{score}");
                }
                Program.WriteTextAt("Skoor salvestatud!", (Console.BufferWidth / 2) - 10, Program.MAP_HEIGHT / 2 + 5);
            }
            catch (Exception ex)
            {
                Program.WriteTextAt($"Viga skoori salvestamisel: {ex.Message}", (Console.BufferWidth / 2) - 20, Program.MAP_HEIGHT / 2 + 5);
            }
        }

        static void ShowHighScores()
        {
            Console.BackgroundColor = Program.bgColor;
            Console.Clear();
            Program.SetCurrentColors(Program.fgColorText, Program.bgColor);

            int centerAlignX = (Console.BufferWidth / 2);
            Program.WriteTextAt("╔════════════════════════╗", centerAlignX - 14, 1);
            Program.WriteTextAt("║    R E K O R D I D     ║", centerAlignX - 14, 2);
            Program.WriteTextAt("╚════════════════════════╝", centerAlignX - 14, 3);

            int scoreListY = 5;

            if (!File.Exists(Program.SCORES_FILENAME))
            {
                Program.WriteTextAt("Rekordeid veel pole.", centerAlignX - 10, scoreListY);
            }
            else
            {
                List<ScoreEntry> scores = new List<ScoreEntry>();
                try
                {
                    string[] lines = File.ReadAllLines(Program.SCORES_FILENAME);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(new char[] { ':' }, 2);
                        if (parts.Length == 2 && int.TryParse(parts[1], out int scoreValue))
                        {
                            scores.Add(new ScoreEntry(parts[0], scoreValue));
                        }
                    }
                    for (int i = 0; i < scores.Count - 1; i++)
                    {
                        for (int j = 0; j < scores.Count - i - 1; j++)
                        {
                            if (scores[j].Score < scores[j + 1].Score)
                            {
                                ScoreEntry temp = scores[j]; scores[j] = scores[j + 1]; scores[j + 1] = temp;
                            }
                        }
                    }
                    int count = 0;
                    foreach (ScoreEntry entry in scores)
                    {
                        string scoreLine = $"{count + 1}. {entry.Name} - {entry.Score}";
                        Program.WriteTextAt(scoreLine, centerAlignX - (scoreLine.Length / 2), scoreListY++);
                        count++;
                        if (count >= 10) break;
                    }
                    if (count == 0) Program.WriteTextAt("Rekordeid veel pole (fail tühi või vale formaat).", centerAlignX - 25, scoreListY);
                }
                catch (Exception ex) { Program.WriteTextAt($"Viga rekordite lugemisel: {ex.Message}", 0, scoreListY++); }
            }
            Program.WriteTextAt("Vajuta Enter menüüsse naasmiseks...", centerAlignX - 17, Math.Max(scoreListY + 2, Program.MAP_HEIGHT - 2));
            Console.CursorVisible = true;
            Console.ReadLine();
            Console.CursorVisible = false;
        }

        static void SelectColorScheme()
        {
            Console.BackgroundColor = Program.bgColor;
            Console.Clear();
            Program.SetCurrentColors(Program.fgColorText, Program.bgColor);

            int centerAlignX = (Console.BufferWidth / 2);
            Program.WriteTextAt("╔════════════════════════╗", centerAlignX - 14, 3);
            Program.WriteTextAt("║   V Ä R V I V A L I K  ║", centerAlignX - 14, 4);
            Program.WriteTextAt("╚════════════════════════╝", centerAlignX - 14, 5);

            string[] schemes = {
                "1. Standard",
                "2. Monokroomne Roheline",
                "3. Monokroomne Punane",
                "4. Monokroomne Kollane"
            };
            int schemeY = 7;
            int schemeItemWidth = 30;

            foreach (string s in schemes)
            {
                Program.WriteTextAt($"   {s.PadRight(schemeItemWidth - 6)}", centerAlignX - schemeItemWidth / 2, schemeY++);
            }
            Program.WriteTextAt("".PadRight(schemeItemWidth, '-'), centerAlignX - schemeItemWidth / 2, schemeY++); // Разделитель
            Program.WriteTextAt("Sinu valik: ", centerAlignX - schemeItemWidth / 2, schemeY);

            Console.CursorVisible = true;
            string choice = Console.ReadLine();
            Console.CursorVisible = false;

            // Позиция для сообщения об ошибке или результате
            int messageY = schemeY + 2; // Через одну строку от ввода

            switch (choice)
            {
                case "1":
                    Program.fgColorPermanentWall = ConsoleColor.Yellow; Program.fgColorDynamicWall = ConsoleColor.DarkGray;
                    Program.fgColorSnake = ConsoleColor.Green; Program.fgColorFood = ConsoleColor.Red;
                    Program.fgColorScissors = ConsoleColor.Cyan; Program.fgColorText = ConsoleColor.White;
                    Program.bgColor = ConsoleColor.Black;
                    break;
                case "2":
                    Program.fgColorPermanentWall = ConsoleColor.Green; Program.fgColorDynamicWall = ConsoleColor.DarkGreen;
                    Program.fgColorSnake = ConsoleColor.Green; Program.fgColorFood = ConsoleColor.Green;
                    Program.fgColorScissors = ConsoleColor.Green; Program.fgColorText = ConsoleColor.Green;
                    Program.bgColor = ConsoleColor.Black;
                    break;
                case "3":
                    Program.fgColorPermanentWall = ConsoleColor.Red; Program.fgColorDynamicWall = ConsoleColor.DarkRed;
                    Program.fgColorSnake = ConsoleColor.Red; Program.fgColorFood = ConsoleColor.Red;
                    Program.fgColorScissors = ConsoleColor.Red; Program.fgColorText = ConsoleColor.Red;
                    Program.bgColor = ConsoleColor.Black;
                    break;
                case "4":
                    Program.fgColorPermanentWall = ConsoleColor.Yellow; Program.fgColorDynamicWall = ConsoleColor.DarkYellow;
                    Program.fgColorSnake = ConsoleColor.Yellow; Program.fgColorFood = ConsoleColor.Yellow;
                    Program.fgColorScissors = ConsoleColor.Yellow; Program.fgColorText = ConsoleColor.Yellow;
                    Program.bgColor = ConsoleColor.Black;
                    break;
                default:
                    Program.WriteTextAt("Vale valik. Värve ei muudetud.", centerAlignX - 15, messageY);
                    messageY++; // Сдвигаем Y для следующего ReadLine, если понадобится
                    break;
            }
            Console.BackgroundColor = Program.bgColor;
            // Console.Clear(); // Убрал полный Clear, т.к. сообщение может быть ниже
            Program.SetCurrentColors(Program.fgColorText, Program.bgColor);
            Program.WriteTextAt("Värviskeem rakendatud. Vajuta Enter...", centerAlignX - 18, messageY);
            Console.CursorVisible = true;
            Console.ReadLine();
            Console.CursorVisible = false;
        }
    }
}