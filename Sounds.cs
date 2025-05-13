using System;
using System.IO;
using System.Media;

namespace Snake
{
    // Класс Sounds: Управляет загрузкой и воспроизведением звуковых эффектов и музыки в игре.
    class Sounds
    {
        private SoundPlayer playerEat;
        private SoundPlayer playerGameOver;
        private SoundPlayer playerBackground;
        private bool backgroundPlaying = false;

        public Sounds(string baseSoundPath)
        {
            string eatPath = Path.Combine(baseSoundPath, "eat.wav");
            string gameOverPath = Path.Combine(baseSoundPath, "gameover.wav");
            string backgroundPath = Path.Combine(baseSoundPath, "background.wav");

            try
            {
                if (!Directory.Exists(baseSoundPath))
                {
                    return;
                }

                if (File.Exists(eatPath)) playerEat = new SoundPlayer(eatPath);
                else Console.Error.WriteLine($"Helifaili '{eatPath}' ei leitud.");

                if (File.Exists(gameOverPath)) playerGameOver = new SoundPlayer(gameOverPath);
                else Console.Error.WriteLine($"Helifaili '{gameOverPath}' ei leitud.");

                if (File.Exists(backgroundPath)) playerBackground = new SoundPlayer(backgroundPath);
                else Console.Error.WriteLine($"Helifaili '{backgroundPath}' ei leitud.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Viga helide laadimisel: {ex.Message}");
            }
        }

        public void PlayEat()
        {
            try { playerEat?.Play(); }
            catch (Exception ex) { Console.Error.WriteLine($"Viga söömise heli mängimisel: {ex.Message}"); }
        }

        public void PlayGameOver()
        {
            try
            {
                StopBackground(); // Сначала остановить фон
                playerGameOver?.PlaySync(); // PlaySync чтобы звук успел проиграться перед закрытием или переходом
            }
            catch (Exception ex) { Console.Error.WriteLine($"Viga mängu lõpu heli mängimisel: {ex.Message}"); }
        }

        public void PlayBackground()
        {
            if (playerBackground != null && !backgroundPlaying)
            {
                try
                {
                    playerBackground.PlayLooping();
                    backgroundPlaying = true;
                }
                catch (Exception ex) { Console.Error.WriteLine($"Viga taustamuusika mängimisel (fail: {playerBackground?.SoundLocation}): {ex.Message}"); }
            }
        }

        public void StopBackground()
        {
            if (playerBackground != null && backgroundPlaying)
            {
                try
                {
                    playerBackground.Stop();
                    backgroundPlaying = false;
                }
                catch (Exception ex) { Console.Error.WriteLine($"Viga taustamuusika peatamisel: {ex.Message}"); }
            }
        }
    }
}
