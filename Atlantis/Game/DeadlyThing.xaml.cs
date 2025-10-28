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
                Sounds fishSfx = new Sounds();
                fishSfx.PlaySfx(@"Assets\Sounds\Sfx\Fish.mp3");
                Scene.DestroyControl(player);
            }
        }
    }
}