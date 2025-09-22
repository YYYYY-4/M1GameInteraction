using Box2dNet.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
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
