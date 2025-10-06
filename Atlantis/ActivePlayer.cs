using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis
{
    internal class ActivePlayer
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ActivePlayer(PlayerSave save)
        {
            _name = save.Name;
        }
    }
}
