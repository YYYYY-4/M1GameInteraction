namespace Atlantis.Game
{
    /// <summary>
    /// Template for GameControl
    /// </summary>
    public partial class DeadlyThing : WaterGameControl
    {
        public DeadlyThing()
        {
            InitializeComponent();
        }

        public override void OnUpdate(float dt)
        {
            if (IsInWater())
            {
                UpdateWaterForces(Shape0);
            }
        }

        public override void OnSensorStart(GameShape sensor, GameShape visitor)
        {
            base.OnSensorStart(sensor, visitor);

            if (visitor.Control is Player player)
            {
                Scene.DestroyControl(player);
            }
        }
    }
}