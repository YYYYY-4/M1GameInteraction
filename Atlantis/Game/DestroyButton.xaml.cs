using Atlantis.Box2dNet;
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

        private bool IsVisitor(GameShape visitor)
        {
            return visitor.Control is not WaterArea;
        }

        public override void OnSensorStart(GameShape sensor, GameShape visitor)
        {
            if (!IsVisitor(visitor))
            {
                return;
            }

            SensorCount += 1;
            UpdateColor();

            if (Targets != null)
            {
                foreach (var control in _targets)
                {
                    Scene.DestroyControl(control);
                }
                _targets = null!;
            }
        }

        public override void OnSensorEnd(GameShape sensor, GameShape visitor)
        {
            if (!IsVisitor(visitor))
            {
                return;
            }

            SensorCount -= 1;
            UpdateColor();
        }

        private List<GameControl> _targets = [];

        public object? Targets
        {
            get
            {
                return _targets?.AsReadOnly();
            }
            set
            {
                if (value is GameControl c)
                {
                    _targets = [c];
                }
                else if (value is GameControl[] a)
                {
                    _targets = [..a];
                }
                else
                {
                    throw new ArgumentException("Invalid Targets value");
                }
            }
        }
    }
}
