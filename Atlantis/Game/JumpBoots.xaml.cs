namespace Atlantis.Game
{
    /// <summary>
    /// Template for GameControl
    /// </summary>
    public partial class JumpBoots : GameControl
    {
        public JumpBoots()
        {
            InitializeComponent();
        }

        public void PickUp(Player player)
        {
            if (player.HasItem == false)
            {
                Scene.DestroyControl(this);
                player.HasItem = true;
                player.ItemType = "JumpBoots";
            }
        }

        public override void OnSensorStart(GameShape sensor, GameShape visitor)
        {
            if (visitor.Control is Player player)
            {
                PickUp(player);
            }
        }
    }
}