using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis
{
    internal class Score
    {
        private string _level;
        private int _score;
        private int tijd = 60; //temp
        private int collectables = 4; //temp

        public int score
        {
            get { return _score; }
            set { _score = value; }
        }

        public Score(string level)
        {
        _level = level;
        }

        public void Calculation ()
        {
            int MaxScore = 10000;
            _score = MaxScore - tijd * 10 + collectables * 100;
        }
    }
}
