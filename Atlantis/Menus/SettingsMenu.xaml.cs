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
        private string MusicSettings = "MusicSettings.txt";
        private string SfxSettings = "SfxSettings.txt";
        public SettingsMenu()
        {
            InitializeComponent();
            if (!File.Exists(MusicSettings))
            {
                File.WriteAllText(MusicSettings, "50");
            }
            if (!File.Exists(SfxSettings))
            {
                File.WriteAllText(SfxSettings, "50");
            }
            MusicSlider.Value = double.Parse(File.ReadAllText(MusicSettings));
            SfxSlider.Value = double.Parse(File.ReadAllText(SfxSettings));
            
        }
        public void OpslaanSettings()
        {
            File.WriteAllText(MusicSettings, MusicSlider.Value.ToString());
            File.WriteAllText(SfxSettings, SfxSlider.Value.ToString());
        }
    }
}