namespace Atlantis.Game
{
    /// <summary>
    /// Template for GameControl
    /// </summary>
    public partial class DoorKey : GameControl
    {
        public DoorKey()
        {
            InitializeComponent();
        }

        private void PickUp()
        {
            EndDoor.OpenDoor();
            Scene.DestroyControl(this);
        }

        public override void OnSensorStart(GameShape sensor, GameShape visitor)
        {
            if (visitor.Control is Player player)
            {
                PickUp();
            }
        }
    }
}