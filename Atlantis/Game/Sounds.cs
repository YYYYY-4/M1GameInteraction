using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.IO;

namespace Atlantis.Game
{
    public class Sounds
    {
        private static MediaPlayer mainMusic = new MediaPlayer();
        private static bool isInitialized = false;
        
        private MediaPlayer loopedSfxPlayer;

        //Internal function for deciding sound volume
        private static double SoundVolume(string filepath)
        {
            if (File.Exists(filepath))
                return double.Parse(File.ReadAllText(filepath)) / 100;
            else
                return 0.5;
        }

        //-------------Music----------------------
        //Plays soundtrack.mp3 and loops the music
        public static void PlayMainMusicLoop()
        {
            //loops music
            if (!isInitialized)
            {
                mainMusic.MediaEnded += (s, e) =>
                {
                    mainMusic.Position = TimeSpan.Zero;
                    mainMusic.Volume = SoundVolume("MusicSettings.txt");
                    mainMusic.Play();
                };
                isInitialized = true;
            }
            //Plays music
            mainMusic.Open(new Uri("/Assets/Sounds/Musi/Soundtrack.mp3"));
            mainMusic.Volume = SoundVolume("MusicSettings.txt");
            mainMusic.Play();
        }

        //stops Music
        public static void StopMainMusicLoop() { mainMusic.Stop(); }

        //Pauses 
        public static void PauseMainMusicLoop() { mainMusic.Pause(); }

        //continues the music (done after it has been pauzed or stopped)
        public static void ContinueMainMusicLoop() { mainMusic.Play(); }

        //Closes/unloads the musicloop 
        public static void CloseMainMusicLoop() 
        {
            mainMusic.Stop();
            mainMusic.Close();
        }

        //updates volume (used in SettingsMenu)
        public static void UpdateMusicVolume()
        {
            mainMusic.Volume = SoundVolume("MusicSettings.txt");
        }

        
        //-------------sfx----------------------
        
        //play any sound effect
        public void PlaySfx(string filePath)
        {
            MediaPlayer sfxPlayer = new MediaPlayer();
            sfxPlayer.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
            sfxPlayer.Volume = SoundVolume("SfxSettings");
            sfxPlayer.Play();

            // Cleanup when finished
            sfxPlayer.MediaEnded += (s, e) =>
            {
                sfxPlayer.Close();
            };
        }

        // play looping sound effect
        public void PlaySfxLoop(string filePath)
        {
            loopedSfxPlayer = new MediaPlayer();
            loopedSfxPlayer.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
            loopedSfxPlayer.Volume = SoundVolume("SfxSettings");

            loopedSfxPlayer.MediaEnded += (s, e) =>
            {
                loopedSfxPlayer.Position = TimeSpan.Zero;
                loopedSfxPlayer.Play();
            };

            loopedSfxPlayer.Play();
        }

        // closes the looping sfx
        public void CloseSfxLoop()
        {
            loopedSfxPlayer.Stop();
            loopedSfxPlayer.Close();
        }
    }
}
