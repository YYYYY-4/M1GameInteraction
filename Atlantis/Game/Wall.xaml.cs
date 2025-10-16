
using System.Windows;
using System.Windows.Shapes;

namespace Atlantis.Game
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    
    public partial class Wall : GameControl
    {
        private Rectangle WallRect;

        public int NCHANGEWIDTH = 0;

        public Wall()
        {
            InitializeComponent();

            WallRect = (Rectangle)Content;
            WallRect.Width = Width;
            WallRect.Height = Height;
        }

        public override void OnBeforeLoadControl()
        {
            base.OnBeforeLoadControl();

            WallRect = (Rectangle)Content;
            WallRect.Width = Width;
            WallRect.Height = Height;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (DesignerMode)
            {
                if (e.Property == WidthProperty)
                {
                    if (TMP != WallRect)
                    {
                        int x = 0;
                    }
                    WallRect.Width = Width;
                }
                else if (e.Property == HeightProperty)
                {
                    WallRect.Height = Height;
                }
            }
        }
    }
}
