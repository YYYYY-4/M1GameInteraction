namespace Atlantis.Game
{
    /// <summary>
    /// Template for GameControl
    /// </summary>
    public partial class DeadlyThing : GameControl
    {
        public DeadlyThing()
        {
            InitializeComponent();
        }

        public override void OnContactStart(GameShape shape, GameShape contact)
        {
            base.OnContactStart(shape, contact);

            if (contact.Control is Player player)
            {
                Scene.DestroyControl(player);
            }
        }

        public override void OnContactEnd(GameShape shape, GameShape contact)
        {
            base.OnContactEnd(shape, contact);
        }
    }
}