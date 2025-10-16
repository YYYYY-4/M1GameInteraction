using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis
{
    internal class GameScore
    {
        private string _level;
        private int _score;
        private int tijd = 60; //temp
        private int collectables = 4; //temp

        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }

        public int Collectables
        {
            get { return collectables; }
            set { collectables = value; }
        }

        public GameScore(string level)
        {
        _level = level;
        }

        public void Calculation ()
        {
            int maxScore = 10000;
            _score = maxScore - tijd * 10 + collectables * 100;
        }
    }
}
