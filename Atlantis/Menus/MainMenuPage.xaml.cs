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