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
    /// Interaction logic for PlayerNameInput.xaml
    /// </summary>
    public partial class PlayerNameInput : Page
    {
        private MainWindow _window;
        private int _saveSlot;
        private string _name;

        public PlayerNameInput(MainWindow window, int saveSlot)
        {
            InitializeComponent();
            _window = window;
            _saveSlot = saveSlot;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _name = nameInput.Text;
        }

        private void Submit_click(object sender, RoutedEventArgs e)
        {
            if (_name != null)
            {
                PlayerSave save = new PlayerSave(_name, _saveSlot);
                save.Save();
                _window.PushPage(new LevelSelect(_window, save));
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _window.GoBack();
        }
    }
}
