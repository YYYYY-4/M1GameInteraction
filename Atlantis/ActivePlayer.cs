using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis
{
    internal class ActivePlayer
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public ActivePlayer(PlayerSave save)
        {
            name = save.Name;
        }
    }
}
