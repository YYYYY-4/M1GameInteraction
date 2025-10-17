using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
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
    /// Interaction logic for SettingsMenu.xaml
    /// </summary>
    public partial class SettingsMenu : Page
    {
        //devines the paths for the txtfiles
        private string MusicSettings = "MusicSettings.txt";
        private string SfxSettings = "SfxSettings.txt";
        private MainWindow _window;

        public SettingsMenu(MainWindow window)
        {
            InitializeComponent();
            _window = window;
            //Creates txtfiles for the settings if they dont exist
            //these files are universal and not per player
            //it adds one txtfile per slider
            //gives them a base value of 50
            if (!File.Exists(MusicSettings))
            {
                File.WriteAllText(MusicSettings, "50");
            }
            if (!File.Exists(SfxSettings))
            {
                File.WriteAllText(SfxSettings, "50");
            }
            //Sets the value for the slider with what's in the txtfiles
            MusicSlider.Value = double.Parse(File.ReadAllText(MusicSettings));
            SfxSlider.Value = double.Parse(File.ReadAllText(SfxSettings));
            _window = window;
        }

        //saves settings to the txtfiles
        public void OpslaanSettings_Button_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(MusicSettings, MusicSlider.Value.ToString());
            File.WriteAllText(SfxSettings, SfxSlider.Value.ToString());
            _window.GoBack();
        }
    }
}