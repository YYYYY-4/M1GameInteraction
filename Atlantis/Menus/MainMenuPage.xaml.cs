using System.Windows;
using System.Windows.Controls;
using Atlantis.Game;
using Atlantis.Menus;

namespace Atlantis.Scene;

public partial class MainMenuPage : Page
{
    private Page _page;
    private MainWindow _window;
    
    // Window is necessary for GamePage 
    public MainMenuPage(MainWindow window)
    {
        InitializeComponent();
        _window = window;

        // Boid simulation on main menu, for temporary demonstration
        var sim = new BoidSimTest();
        sim.Content = null;
        Grid.SetRowSpan(sim._canvas, int.MaxValue);
        Grid.SetColumnSpan(sim._canvas, int.MaxValue);
        Grid.SetZIndex(sim._canvas, -10);
        ((Grid)Content).Children.Add(sim._canvas);
    }

    private void Start_Button_Click(object sender, RoutedEventArgs e)
    {
        _window.PushPage(new PlayerSelect(_window));
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        _window.PushPage(new SettingsMenu(_window));
    }

    private void Quit_Button_Click(object sender, RoutedEventArgs e)
    {
        // Terminates process and tells underlying process quit
        Environment.Exit(0);
    }
}