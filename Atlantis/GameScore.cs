using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis
{
    public class GameScore
    {
        //private string _level;
        private int _score;
        private int tijd = 60; //temp
        private int collectables = 0; //temp

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

        //public GameScore(string level)
        //{
        //_level = level;
        //}

        public int Calculation(float time)
        {
            int maxScore = 10000;
            _score = maxScore - (Convert.ToInt32(time * 50)) + (collectables * 1000);

            return _score;
        }
    }
}
