using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Atlantis.Game;
using System.IO;

namespace Atlantis.Game
{
    public class Music
    {
        private static MediaPlayer _mainMusic = new MediaPlayer();

        //Plays soundtrack.mp3 and loops the music
        public static void PlayMainMusicLoop()
        {
            //Plays music
            _mainMusic.Open(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Assets\Sounds\Music\Soundtrack.mp3")));
            _mainMusic.Volume = Sounds.SoundVolume("MusicSettings.txt");
            _mainMusic.MediaEnded += new EventHandler(Media_Ended);
            _mainMusic.Play();
        }

        //loops music
        private static void Media_Ended(object sender, EventArgs e)
        {
            _mainMusic.Position = TimeSpan.Zero;
            _mainMusic.Play();
        }

        //stops Music
        public static void StopMainMusicLoop() { _mainMusic.Stop(); }

        //Pauses 
        public static void PauseMainMusicLoop() { _mainMusic.Pause(); }

        //continues the music (done after it has been pauzed or stopped)
        public static void ContinueMainMusicLoop() { _mainMusic.Play(); }

        //Closes the musicloop 
        public static void CloseMainMusicLoop() 
        {
            _mainMusic.Stop();
            _mainMusic.Close();
        }

        //updates volume (used in SettingsMenu)
        public static void UpdateMusicVolume()
        {
            _mainMusic.Volume = Sounds.SoundVolume("MusicSettings.txt");
        }
    }
}
