using System.Windows;

namespace Atlantis.Game
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    
    public partial class Wall : GameControl
    {
        public Wall()
        {
            InitializeComponent();

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
