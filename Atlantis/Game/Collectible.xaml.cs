using System.Windows.Media;

namespace Atlantis.Game
{
    /// <summary>
    /// Template for GameControl
    /// </summary>
    public partial class Collectible : GameControl
    {
        public ImageSource CollectibleImageSource
        {
            get => img.Source;
            set
            {
                img.Source = value;
            }
        }

        public Collectible()
        {
            InitializeComponent();
        }

        bool _debounce = false;
        public override void OnSensorStart(GameShape sensor, GameShape visitor)
        {
            if (_debounce)
            {
                return;
            }

            if (visitor.Control is Player player)
            {
                Scene.GamePage.Score.Collectables += 1;
                Scene.DestroyControl(this);
                _debounce = true;
            }
        }
    }
}