using Atlantis.Game;
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
        private static string s_fileName = "HighscoreList.csv";

        public HighscorePage()
        {
            InitializeComponent();

            // Needed otherwise wpf can not find this namespace or whatever...
            DataContext = this;


            //PlayerSave playerSave = new PlayerSave("Ferry", 1);
            //ActivePlayer player = new ActivePlayer(playerSave);


            // Variabele, Have to get these from other people
            // For now they are random
            //int score = Random.Shared.Next(10000);
            //string name = player.Save.Name;
            int level = Random.Shared.Next(1, 13);

            int score = 0;
            // Puts the score in the file, if file does not exist creates the file
            //AddRecord(level, score, name);


            // Gets the data from the .csv file
            Scoreboard = ReadData(level);
        }


        /// <summary>
        /// Creates a file to keep record of the scores
        /// </summary>
        /// <param name="fileName"></param>
        private static void Create()
        {
            var file = File.Create(s_fileName);
            file.Close();
        }

        /// <summary>
        /// Adds the Name + Score to the given file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="score"></param>
        /// <param name="fileName"></param>
        public static void AddRecord(int level, int score, string name)
        {
            
            if (!File.Exists(s_fileName))
                Create();

            var newLine = string.Format("{0},{1},{2}\n", level, score, name);
            File.AppendAllText(s_fileName, newLine);
        }

        /// <summary>
        /// Reads the data from .csv file, Makes a object with the collected data
         /// </summary>
        /// <param name="activelevel"></param>
        /// <returns></returns>
        public static List<HighscoreRecord> ReadData(int activelevel)
        {
            List<HighscoreRecord> records = [];

            string[] HighscoreList = File.ReadAllLines(s_fileName);

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

                int level = Convert.ToInt32(values[0]);
                if (level == activelevel)
                {
                    int score = Convert.ToInt32(values[1]);
                    string name = values[2];

                    records.Add(new HighscoreRecord()
                    {
                        Level = level,
                        Score = score,
                        Name = name
                    });
                }
            }
            records.Sort(CompareScores);

            return records;
        }

        /// <summary>
        /// Comparing method of two HighscoreRecord objects
        /// </summary>
        /// <param name="record1"></param>
        /// <param name="record2"></param>
        /// <returns></returns>
        private static int CompareScores(HighscoreRecord record1, HighscoreRecord record2)
        {
            if (record1.Score > record2.Score) 
                return -1;
            if (record1.Score < record2.Score) 
                return 1;
            
            return 0;
        }

        // Object list om mee te geven aan de highscore list
        public List<HighscoreRecord> Scoreboard 
        { 
            get;
            set;
        }

        //Temp
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlayerSave playerSave = new PlayerSave("Ferry", 1);

            AddRecord(Random.Shared.Next(1, 13), 9999 , playerSave.Name);
        }
    }
}
