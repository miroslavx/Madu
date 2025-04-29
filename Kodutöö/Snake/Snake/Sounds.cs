// Sounds.cs
using System;
using System.IO; // Для работы с путями к файлам
using System.Media; // Для проигрывания звуков .wav
using System.Threading.Tasks; // Добавлено для возможного Task.Delay

namespace Snake
{
    class Sounds
    {
        private SoundPlayer eatSound;
        private SoundPlayer gameOverSound;
        private SoundPlayer backgroundMusic;
        private string soundsFolder;
        private bool backgroundPlaying = false; // Флаг для отслеживания, должна ли музыка играть

        public Sounds()
        {
            soundsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zvuki");

            // Загрузка звуков с проверкой на null и ошибками
            eatSound = LoadSound("eat.wav", "Söömise");
            gameOverSound = LoadSound("gameover.wav", "Mängu lõpu");
            backgroundMusic = LoadSound("background.wav", "Taustamuusika");
        }

        // Вспомогательный метод для загрузки звука
        private SoundPlayer LoadSound(string fileName, string soundName)
        {
            try
            {
                string filePath = Path.Combine(soundsFolder, fileName);
                if (File.Exists(filePath))
                {
                    SoundPlayer player = new SoundPlayer(filePath);
                    player.LoadAsync(); // Предзагрузка
                    return player;
                }
                else
                {
                    DebugPrint($"Hoiatus: Helifaili ei leitud: {filePath}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                DebugPrint($"Hoiatus: {soundName} heli laadimine ebaõnnestus. Viga: {ex.Message}");
                return null;
            }
        }

        public void PlayEatSound()
        {
            try
            {
                if (eatSound != null)
                {
                    // 1. Останавливаем фон, если он играл
                    bool wasPlaying = backgroundPlaying;
                    if (wasPlaying)
                    {
                        StopBackgroundMusic(); // Используем наш метод для корректной остановки и сброса флага
                    }

                    // 2. *** ИЗМЕНЕНИЕ: Используем PlaySync() для блокирующего воспроизведения ***
                    eatSound.PlaySync(); // Ждем, пока звук закончится

                    // 3. Перезапускаем фон, если он должен был играть
                    if (wasPlaying)
                    {
                        PlayBackgroundMusic(); // Используем наш метод для корректного запуска и установки флага
                    }
                }
            }
            catch (Exception ex)
            {
                DebugPrint($"Hoiatus: Söömise heli esitamine ebaõnnestus: {ex.Message}");
                // Если возникла ошибка во время PlaySync, нужно убедиться,
                // что музыка все равно перезапустится, если она играла.
                if (backgroundPlaying)
                { // Проверяем флаг напрямую, т.к. Stop мог не сработать из-за ошибки
                    PlayBackgroundMusic();
                }
            }
        }

        public void PlayGameOverSound()
        {
            try
            {
                // Останавливаем фон перед звуком проигрыша
                StopBackgroundMusic(); // Используем наш метод

                gameOverSound?.Play(); // Используем Play() т.к. после этого игра заканчивается, блокировка не нужна
            }
            catch (Exception ex)
            {
                DebugPrint($"Hoiatus: Mängu lõpu heli esitamine ebaõnnestus: {ex.Message}");
            }
        }

        public void PlayBackgroundMusic()
        {
            try
            {
                if (backgroundMusic != null && !backgroundPlaying)
                {
                    backgroundMusic.PlayLooping();
                    backgroundPlaying = true;
                }
            }
            catch (Exception ex)
            {
                DebugPrint($"Hoiatus: Taustamuusika esitamine ebaõnnestus: {ex.Message}");
            }
        }

        public void StopBackgroundMusic()
        {
            try
            {
                if (backgroundMusic != null && backgroundPlaying)
                {
                    backgroundMusic.Stop();
                    backgroundPlaying = false;
                }
            }
            catch (Exception ex)
            {
                DebugPrint($"Hoiatus: Taustamuusika peatamine ebaõnnestus: {ex.Message}");
            }
        }

        // Метод для вывода отладочных сообщений
        private static void DebugPrint(string message)
        {
            // Раскомментируй, если нужно видеть сообщения об ошибках загрузки/воспроизведения
            // Console.WriteLine(message);
        }
    }
}