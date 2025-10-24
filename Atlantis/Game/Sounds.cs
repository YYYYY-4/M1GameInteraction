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
        private static double musicVolume = double.Parse(File.ReadAllText("MusicSettings.txt")) / 100;
        private double sfxVolmume = double.Parse(File.ReadAllText("SfxSettings")) / 100; 

        //Plays soundtrack.mp3 and loops the music
        public static void PlayMainMusicLoop()
        {
            //loops music
            if (!isInitialized)
            {
                mainMusic.MediaEnded += (s, e) =>
                {
                    mainMusic.Position = TimeSpan.Zero;
                    mainMusic.Volume = musicVolume;
                    mainMusic.Play();
                };
                isInitialized = true;
            }
            //Plays music
            mainMusic.Open(new Uri("/Assets/Sounds/Musi/Soundtrack.mp3"));
            mainMusic.Volume = musicVolume;
            mainMusic.Play();
        }

        //stops Music (not unload)
        public static void StopMainMusicLoop()
        {
            mainMusic.Stop();
        }

        //Pauses 
        public static void PauseMainMusicLoop()
        {
            mainMusic.Pause();
        }

        //continues the music (done after it has been pauzed or stopped)
        public static void ContinueMainMusicLoop()
        {
            mainMusic.Play();
        }

        //Closes/Unloads the music so it doesnt take up resources 
        public static void CloseMainMusicLoop()
        {
            mainMusic.Close();
        }

        //updates volume (used in SettingsMenu)
        public static void UpdateMusicVolume()
        {
            musicVolume = double.Parse(File.ReadAllText("MusicSettings.txt"));
            mainMusic.Volume = musicVolume;
        }
    }
}
