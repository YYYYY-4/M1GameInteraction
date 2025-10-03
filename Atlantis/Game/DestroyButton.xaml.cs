using System.Windows.Media;

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
