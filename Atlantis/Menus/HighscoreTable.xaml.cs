using System;
using System.Collections.Generic;
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

            Voertuig Auto = new Voertuig();

            new Motor();

            Auto.setmerk("Ford");
            string merknaam = Auto.getmerk();
            Console.WriteLine( merknaam );
        }
    }

    public class Voertuig
    {
        protected string merk;

        public Voertuig()
        {
            merk = "merkloos";
        }

        public string getmerk()
        {
            return merk;
        }

        public void setmerk(string merk)
        {
            merk = this.merk;
        }

    }

    public class Motor : Voertuig
    {
        public int pk;

        public Motor()
        {
            merk = "BMW";
            pk = 999;
        }
    }
}
