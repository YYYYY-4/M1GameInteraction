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
        
        private MediaPlayer _loopedSfxPlayer;

        //function for deciding sound volume
        public static double SoundVolume(string filepath)
        {
            if (File.Exists(filepath))
                return double.Parse(File.ReadAllText(filepath)) / 100;
            else
                return 0.5;
        }
        
        //play any sound effect
        public void PlaySfx(string filePath)
        {
            MediaPlayer sfxPlayer = new MediaPlayer();
            sfxPlayer.Open(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\", filePath)));
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
            _loopedSfxPlayer = new MediaPlayer();
            _loopedSfxPlayer.Open(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\", filePath)));
            _loopedSfxPlayer.Volume = SoundVolume("SfxSettings");

            _loopedSfxPlayer.MediaEnded += new EventHandler(Media_Ended);

            _loopedSfxPlayer.Play();
        }

        //function for looping loopedsfxplayer
        private void Media_Ended(object sender, EventArgs e)
        {
            _loopedSfxPlayer.Position = TimeSpan.Zero;
            _loopedSfxPlayer.Play();
        }

        // closes the looping sfx
        public void CloseSfxLoop()
        {
            _loopedSfxPlayer.Stop();
            _loopedSfxPlayer.Close();
        }
    }
}
