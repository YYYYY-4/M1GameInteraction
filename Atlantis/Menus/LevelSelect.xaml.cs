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

        private void C1L1_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 0));
        }

        private void C1L2_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 1));
        }

        private void C1L3_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 2));
        }

        private void C1L4_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 3));
        }

        private void C2L1_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 4));
        }

        private void C2L2_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 5));
        }

        private void C2L3_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 6));
        }

        private void C2L4_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 7));
        }

        private void C3L1_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 8));
        }

        private void C3L2_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 9));
        }

        private void C3L3_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 10));
        }

        private void C3L4_Click(object sender, RoutedEventArgs e)
        {
            _window.PushPage(new GamePage(_window, _save, 11));
        }
    }
}
