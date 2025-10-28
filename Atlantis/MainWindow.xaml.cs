using Atlantis.Game;
using Atlantis.Menus;
using Atlantis.Menus;
using Atlantis.Scene;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Windows;
using System.Windows.Controls;

namespace Atlantis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Page _page;
        private Canvas _canvas;
        private Grid _grid;
        private SettingsMenu _menu;

        public BoidSimulationCanvas BoidSimulation;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            BoidSimulation = new BoidSimulationCanvas();

            PushPage(new MainMenuPage(this));
            
            Music.PlayMainMusicLoop();
        }

        public List<Page> PageHistory { get; } = new List<Page>();

        public void GoBack()
        {
            Content = PageHistory[PageHistory.Count - 2];
            if ((Content as Page)?.Content is Grid grid)
            {
                BoidSimulation.MountPanel(grid);
            }

            if (PageHistory.Count != 1)
            {
                PageHistory.RemoveAt(PageHistory.Count - 1);
            }

            Trace.WriteLine("AfterBack: " + string.Join(", ", PageHistory));
        }

        public void GoBackToType<PageType>()
        {
            for (int i = PageHistory.Count - 1; i >= 0; i--)
            {
                if (PageHistory[i] is PageType)
                {
                    GoBackToIndex(i);
                    break;
                }
            }
        }

        private void GoBackToIndex(int index)
        {
            Page page = PageHistory[index];
            PageHistory.RemoveRange(index + 1, PageHistory.Count - (index + 1));
            Content = page;

            if (page.Content is Grid grid)
            {
                BoidSimulation.MountPanel(grid);
            }
        }

        public void PushPage(Page page)
        {
            page.ClearValue(Page.HeightProperty);
            page.ClearValue(Page.WidthProperty);

            PageHistory.Add(page);
            Content = page;

            if (page.Content is Grid grid)
            {
                BoidSimulation.MountPanel(grid);
            }

            Trace.WriteLine("AfterPush: " + string.Join(", ", PageHistory));
        }
    }
}


