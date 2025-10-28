using System.Windows.Media;

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

        public override string GetDisplayName()
        {
            return "Jump Boots";
        }

        public override ImageSource GetIconResource()
        {
            return (ImageSource)FindResource("JumpBoots");
        }
    }
}