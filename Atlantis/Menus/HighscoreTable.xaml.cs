using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

namespace Atlantis.Menus
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class HighscorePage : Page
    {
        public HighscorePage()
        {
            InitializeComponent();
            PlayerSave playerSave = new PlayerSave("Ferry", 1);
            ActivePlayer player = new ActivePlayer(playerSave);


            // Variabele
            int score = Random.Shared.Next(10000);
            string name = player.Save.Name;
            string fileName = "HighscoreList.csv";
            StreamReader reader = null;

            // Creeërt .csv file als die nog niet bestaat
            if (!File.Exists(fileName))
            {
                var file = File.Create(fileName);
                file.Close();
            }

            if (File.Exists(fileName))
            {
                // Stopt variabele in de .csv file
                var newLine = string.Format("{0},{1}\n", name, score);
                File.AppendAllText(fileName, newLine);
                string[] HighscoreList = File.ReadAllLines(fileName);

                //Haalt de data uit de .csv file
                reader = new StreamReader(File.OpenRead(fileName));

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                }

                reader.Close();

                // of
                List<List<string>> allValues = [];
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
            }
            else Console.WriteLine("The file doesn't exist")

            Console.ReadKey(true);
        }
    }
}
