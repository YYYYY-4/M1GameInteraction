using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Atlantis.Game;
using Atlantis.Scene;

namespace Atlantis.Menus;

/// <summary>
/// This page shows the GameScenes in the Scene folder.
/// </summary>
public partial class GamePage : Page
{
    MainWindow _window;
    Canvas _canvas;
    GameScene _scene;

    public GameScore Score = new GameScore();

    private PlayerSave _save;
    private int _level;
    
    public GamePage(MainWindow window, PlayerSave save, int level)
    {
        InitializeComponent();
        window.ChangeBackground("/Assets/placeholder.png");
        _window = window;
        _save = save;
        _level = level;
        LoadScene<DemoLevel>();

        Focusable = true;
        Focus();

        //Overlay.Visibility = Visibility.Hidden;
    }

    /// <summary>
    /// Everything that happens when you win a _level
    /// </summary>
    public void Win()
    {
        int score = Score.Calculation(_scene.Time);
        HighscorePage.AddRecord(_level, score, _save.Name);

        HigscoreTable.ItemsSource = HighscorePage.ReadData(_level);

        Overlay.Visibility = Visibility.Visible;
    }

    /// <typeparam name="T">Scene which inherits Page and defines a Canvas at it's root.</typeparam>
    public void LoadScene<T>() where T : Page
    {
        if (_scene != null)
        {
            _scene.Destroy();
            RootGrid.Children.Remove(_canvas);
        }

        Score = new GameScore();
        var page = new DemoLevel();
        _canvas = (Canvas)page.Content;
        _scene = new GameScene(_window, this, _canvas);

        page.Content = null;

        // game in background
        Panel.SetZIndex(_canvas, int.MinValue); 

        // use entire grid
        Grid.SetColumnSpan(_canvas, int.MaxValue);
        Grid.SetRowSpan(_canvas, int.MaxValue);

        RootGrid.Children.Add(_canvas);

        SetPaused(false);
    }

    public void SetPaused(bool paused)
    {
        _scene.Paused = paused;
        Overlay.Visibility = _scene.Paused ? Visibility.Visible : Visibility.Hidden;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape || e.Key == Key.P)
        {
            SetPaused(!_scene.Paused);
        }
        else if (e.Key == Key.F1)
        {
            LoadScene<DemoLevel>();
        }
    }

    private void Button_Continue(object sender, RoutedEventArgs e)
    {
        SetPaused(false);
    }

    private void Button_Restart(object sender, RoutedEventArgs e)
    {
        LoadScene<DemoLevel>();
    }

    private void Button_LevelSelect(object sender, RoutedEventArgs e)
    {
        _window.GoBack();
    }

    private void Button_Settings(object sender, RoutedEventArgs e)
    {
        _window.PushPage(new SettingsMenu(_window));
    }

    private void Button_MainMenu(object sender, RoutedEventArgs e)
    {
        _window.GoBackToType<MainMenuPage>();
    }

    private void Button_Exit(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}