using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis
{
    internal class ActivePlayer
    {
        static ActivePlayer instance;
        
        public PlayerSave Save;

        public ActivePlayer(PlayerSave save)
        {
            Save = save;
        }
    }
}
