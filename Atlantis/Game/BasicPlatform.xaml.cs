using System.Windows.Media;

namespace Atlantis.Game
{
    /// <summary>
    /// Interaction logic for BasicPlatform.xaml
    /// </summary>
    public partial class BasicPlatform : GameControl
    {
        public Brush FillColor { get; set; }

        public BasicPlatform()
        {
            InitializeComponent();
        }
    }
}
