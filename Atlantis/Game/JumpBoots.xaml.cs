namespace Atlantis.Game
{
    /// <summary>
    /// Template for GameControl
    /// </summary>
    public partial class JumpBoots : Item
    {
        public JumpBoots()
        {
            InitializeComponent();
        }

        public override void OnSensorStart(GameShape sensor, GameShape visitor)
        {
            if (visitor.Control is Player player)
            {
                player.Inventory.PickUp(this);
            }
        }
    }
}