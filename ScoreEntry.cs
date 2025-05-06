namespace Snake
{
    // Класс ScoreEntry: Простая структура данных для хранения имени игрока и его счета.
    class ScoreEntry
    {
        public string Name;
        public int Score;

        public ScoreEntry(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}