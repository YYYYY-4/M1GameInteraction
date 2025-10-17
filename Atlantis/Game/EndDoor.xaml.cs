using Atlantis.Menus;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Atlantis.Game
{
    /// <summary>
    /// Template for GameControl
    /// </summary>
    public partial class EndDoor : GameControl
    {
        private float _timer = 0.0f;
        private static bool _isOpening = false;
        private bool _isOpened = false;

        // Ferry, why does the player have 3 hitboxes at once?!?!??!?!?
        private bool _alreadyWon = false;
        

        public EndDoor()
        {
            InitializeComponent();
            DataContext = this;
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (_isOpening)
            {
                _timer = _timer + dt;

                if (_timer >= 1.0f)
                {
                    _isOpened = true;
                    _isOpening = false;
                    Door.Source = (ImageSource)Application.Current.FindResource("DoorOpen");

                }
                else if (_timer >= 0.5f)
                {
                    Door.Source = (ImageSource)Application.Current.FindResource("DoorOpening");
                }
            }
        }

        public static void OpenDoor()
        {
            _isOpening = true;
        }

        public override void OnSensorStart(GameShape sensor, GameShape visitor)
        {
            if (_isOpened && visitor.Control is Player)
            {
                if (!_alreadyWon)
                {
                    Scene.GamePage.Win();
                    _alreadyWon = true;
                }
            }
        }
    }
}