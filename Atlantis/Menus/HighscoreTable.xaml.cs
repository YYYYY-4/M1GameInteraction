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

            int Score = Random.Shared.Next(10000);
            string Name1 = "Ferry";

            if (!File.Exists("example.csv"))
            {
                var file = File.Create("example.csv");
                file.Close();
            }

            var newLine = string.Format("{0},{1}\n", Name1, Score);
            File.AppendAllText("example.csv", newLine);
            string[] HighscoreList = File.ReadAllLines("example.csv");

            //Haalt de data uit de .csv file
            List<List<string>> allValues = [];
            for (int i = 0; i < HighscoreList.Length; i++)
            {
                string Line = HighscoreList[i];
                //Line.Split(",");

                int cursor = 0;
                List<string> values = [];

                for (int j = 0; j < Line.Length; j++)
                {
                    if (Line[j] == ',')
                    {
                        values.Add(Line.Substring(cursor, j - cursor));
                        cursor = j + 1;
                    }
                }
                values.Add(Line.Substring(cursor));

                allValues.Add(values);
            }
        }
    }
}
