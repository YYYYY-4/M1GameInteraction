using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for DestroyButton.xaml
    /// </summary>
    public partial class DestroyButton : GameControl
    {
        public DestroyButton()
        {
            InitializeComponent();
        }

        public override void OnStart()
        {
            UpdateColor();
        }

        public override void OnUpdate(float dt)
        {
        }

        int SensorCount = 0;

        void UpdateColor()
        {
            RButton.Fill = SensorCount == 0 ? Brushes.Green : Brushes.Red;
        }

        public override void OnSensorStart(GameShape sensor, GameShape visitor)
        {
            SensorCount += 1;
            UpdateColor();
        }

        public override void OnSensorEnd(GameShape sensor, GameShape visitor)
        {
            SensorCount -= 1;
            UpdateColor();
        }

        public string Targets { get; set; }
    }
}
