namespace Snake
{
    // Класс ScoreEntry: Для хранения имени игрока и его счета
    class ScoreEntry
    {
        public string Name;  // Имя игрока ( включает кст тег режима игры)
        public int Score;    // Количество очков

        public ScoreEntry(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}