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

namespace Atlantis.Menus
{
    /// <summary>
    /// Interaction logic for LevelSelect.xaml
    /// </summary>
    public partial class LevelSelect : Page
    {
        private MainWindow _window;
        private PlayerSave _save;

        public LevelSelect(MainWindow window, PlayerSave save)
        {
            InitializeComponent();
            _window = window;
            _save = save;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 0));
        }
    }
}
