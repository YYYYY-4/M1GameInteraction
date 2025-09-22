using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
