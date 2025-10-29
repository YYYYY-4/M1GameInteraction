using Atlantis.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
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
                // Needs to be called to reactively change the name of the SaveFile to New Save
                OnPropertyChanged(nameof(SaveName0));
            }
        }
        public string SaveName1
        {
            get { return _save1?.Name ?? "New Save"; }
            set
            {
                _save1.Name = value;
                OnPropertyChanged(nameof(SaveName1));
            }
        }
        public string SaveName2
        {
            get { return _save2?.Name ?? "New Save"; }
            set
            {
                _save2.Name = value;
                OnPropertyChanged(nameof(SaveName2));
            }
        }

        /// <summary>
        /// Loads all existing saveFiles on page creation, save variables are null if saveFile does not exist
        /// </summary>
        /// <param name="window"></param>
        public PlayerSelect(MainWindow window)
        {
            InitializeComponent();
            _window = window;
            DataContext = this;
            _save0 = PlayerSave.Load(0);
            _save1 = PlayerSave.Load(1);
            _save2 = PlayerSave.Load(2);
        }

        /// <summary>
        /// Checks if saveFile0 exists, if not creates a playerNameInput page, if yes creates a levelSelect page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile0_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerSave.Load(0) == null)
                _window.PushPage(new PlayerNameInput(_window, 0));
            else
                _window.PushPage(new LevelSelect(_window, _save0));
        }

        /// <summary>
        /// Checks if saveFile1 exists, if not creates a playerNameInput page, if yes creates a levelSelect page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile1_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerSave.Load(1) == null)
                _window.PushPage(new PlayerNameInput(_window, 1));
            else
                _window.PushPage(new LevelSelect(_window, _save1));
        }

        /// <summary>
        /// Checks if saveFile2 exists, if not creates a playerNameInput page, if yes creates a levelSelect page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile2_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerSave.Load(2) == null)
                _window.PushPage(new PlayerNameInput(_window, 2));
            else
                _window.PushPage(new LevelSelect(_window, _save2));
        }

        private void DeleteSave0_Click(object sender, RoutedEventArgs e)
        {
            if (_save0 != null)
            {
                _save0.Delete();
                SaveName0 = null;
            }
        }

        private void DeleteSave1_Click(object sender, RoutedEventArgs e)
        {
            if (_save1 != null)
            {
                _save1.Delete();
                SaveName1 = null;
            }
        }

        private void DeleteSave2_Click(object sender, RoutedEventArgs e)
        {
            if (_save2 != null)
            {
                _save2.Delete();
                SaveName2 = null;
            }
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

        /// <summary>
        /// Goes back to previous page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Sounds sfxtest = new Sounds();
            sfxtest.PlaySfx(@"Assets\Sounds\Sfx\Thump.mp3");
            _window.GoBack();
        }
    }
}
