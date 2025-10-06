using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Atlantis.Menus
{
    /// <summary>
    /// The logic for HighscorePage
    /// </summary>
    public partial class HighscorePage : Page
    {
        private string _fileName = "HighscoreList.csv";

        public HighscorePage()
        {
            InitializeComponent();
            PlayerSave playerSave = new PlayerSave("Ferry", 1);
            ActivePlayer player = new ActivePlayer(playerSave);


            // Variabele
            int score = Random.Shared.Next(10000);
            string name = player.Save.Name;
            int level = Random.Shared.Next(1, 13);


            // Puts the score in the file, if file does not exist creates the file
            AddRecord(level, score, name);


            //Haalt de data uit de .csv file
            ReadData();
        }

        /// <summary>
        /// Creates a file to keep record of the scores
        /// </summary>
        /// <param name="fileName"></param>
        private void Create()
        {
            var file = File.Create(_fileName);
            file.Close();
        }

        /// <summary>
        /// Adds the Name + Score to the given file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="score"></param>
        /// <param name="fileName"></param>
        private void AddRecord(int level, int score, string name)
        {
            // Creeërt .csv file als die nog niet bestaat
            if (!File.Exists(_fileName))
                Create();

            var newLine = string.Format("{0},{1},{2}\n", level, score, name);
            File.AppendAllText(_fileName, newLine);
        }

        /// <summary>
        /// Reads the data from .csv file, Makes a object with the collected data
        /// </summary>
        /// <returns></returns>
        private List<HighscoreRecord> ReadData()
        {
            List<List<string>> allValues = [];

            string[] HighscoreList = File.ReadAllLines(_fileName);

            for (int i = 0; i < HighscoreList.Length; i++)
            {
                string line = HighscoreList[i];
                int cursor = 0;
                List<string> values = [];

                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j] == ',')
                    {
                        values.Add(line.Substring(cursor, j - cursor));
                        cursor = j + 1;
                    }
                }
                values.Add(line.Substring(cursor));

                allValues.Add(values);
            }


            List<HighscoreRecord> records = [];

            for (int i = 0; i < allValues.Count; i++)
            {
                int level = Convert.ToInt32(allValues[i][0]);
                int score = Convert.ToInt32(allValues[i][1]);
                string name = allValues[i][2];

                records.Add(new HighscoreRecord()
                {
                    Level = level,
                    Score = score,
                    Name = name
                });
            }

            return [];
        }
    }
}
