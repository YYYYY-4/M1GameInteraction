using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for PlayerSelect.xaml
    /// </summary>
    public partial class PlayerSelect : Page, INotifyPropertyChanged
    {
        private MainWindow _window;

        private PlayerSave? _save0;
        private PlayerSave? _save1;
        private PlayerSave? _save2;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string SaveName0
        {
            get { return _save0?.Name ?? "New Save"; }
            set 
            { 
                _save0.Name = value;
                OnPropertyChanged(nameof(SaveName0));
            }
        }
        public string SaveName1
        {
            get { return _save1?.Name ?? "New Save"; }
            set
            {
                _save1.Name = value;
                OnPropertyChanged(nameof(SaveName0));
            }
        }
        public string SaveName2
        {
            get { return _save2?.Name ?? "New Save"; }
            set
            {
                _save2.Name = value;
                OnPropertyChanged(nameof(SaveName0));
            }
        }

        public PlayerSelect(MainWindow window)
        {
            InitializeComponent();
            _window = window;
            DataContext = this;
            _save0 = PlayerSave.Load(0);
            _save1 = PlayerSave.Load(1);
            _save2 = PlayerSave.Load(2);
        }

        private void SaveFile0_Click(object sender, RoutedEventArgs e)
        {
            _window.PageHistory.Add(this);
            if (PlayerSave.Load(0) == null)
                _window.Content = new PlayerNameInput(_window , 0);
            else
                _window.Content = new LevelSelect(_window, _save0);
        }

        private void SaveFile1_Click(object sender, RoutedEventArgs e)
        {
            _window.PageHistory.Add(this);
            if (PlayerSave.Load(1) == null)
                _window.Content = new PlayerNameInput(_window, 1);
            else
                _window.Content = new LevelSelect(_window, _save1);
        }

        private void SaveFile2_Click(object sender, RoutedEventArgs e)
        {
            _window.PageHistory.Add(this);
            if (PlayerSave.Load(2) == null)
                _window.Content = new PlayerNameInput(_window, 2);
            else
                _window.Content = new LevelSelect(_window, _save2);
        }

        private void DeleteSave0_Click(object sender, RoutedEventArgs e)
        {
            _save0.Delete();
            SaveName0 = null;
        }

        private void DeleteSave1_Click(object sender, RoutedEventArgs e)
        {
            _save1.Delete();
            SaveName1 = null;
        }

        private void DeleteSave2_Click(object sender, RoutedEventArgs e)
        {
            _save2.Delete();
            SaveName2 = null;
        }
        protected void OnPropertyChanged(string propertyName)
        {

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}
