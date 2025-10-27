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
    private bool _completed = false;

    public string Level => $"Level {_level+1}";

    public static readonly Dictionary<int, Type> Levels = new()
    {
        { 0, typeof(DemoLevel) },
        { 1, typeof(DemoLevel) },
        { 2, typeof(DemoLevel) },
        { 3, typeof(DemoLevel) },
        { 4, typeof(DemoLevel2) }
    };

    public GamePage(MainWindow window, PlayerSave save, int level)
    {
        InitializeComponent();
        _window = window;
        _save = save;
        _level = level;

        LoadScene();

        Focusable = true;
        Focus();

        UpdatePauseVisible();
    }

    /// <summary>
    /// Everything that happens when you win a _level
    /// </summary>
    public void Win()
    {
        int score = Score.Calculation(_scene.Time);
        Highscores.AddRecord(_level, score, _save.Name);

        HigscoreTable.ItemsSource = Highscores.ReadData(_level);

        _completed = true;
        ContinueButton.Content = "Next Level";
        LevelStatus.Content = "Completed";
        SetPaused(true);

        Overlay.Visibility = Visibility.Visible;
    }

    /// <typeparam name="T">Scene which inherits Page and defines a Canvas at it's root.</typeparam>
    public void LoadScene()
    {

        if (!Levels.TryGetValue(_level, out var levelType))
        {
            return;
        }

        if (_scene != null)
        {
            _scene.Destroy();
            RootGrid.Children.Remove(_canvas);
        }

        var page = (Page)levelType.GetConstructors()[0].Invoke(null);

        Score = new GameScore();

        _canvas = (Canvas)page.Content;
        _scene = new GameScene(_window, this, _canvas);

        _completed = false;
        page.Content = null;

        // game in background
        Panel.SetZIndex(_canvas, int.MinValue);

        // use entire grid
        Grid.SetColumnSpan(_canvas, int.MaxValue);
        Grid.SetRowSpan(_canvas, int.MaxValue);

        RootGrid.Children.Add(_canvas);
        LevelStatus.Content = "Paused";
        ContinueButton.Content = "Continue";

        UpdatePauseVisible();
    }

    private void UpdatePauseVisible()
    {
        if (_scene == null)
        {
            Overlay.Visibility = Visibility.Visible;
        }
        else
        {
            Overlay.Visibility = _scene.Paused ? Visibility.Visible : Visibility.Hidden;
        }
    }

    public void SetPaused(bool paused)
    {
        _scene.Paused = paused;
        UpdatePauseVisible();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (_scene != null && (e.Key == Key.Escape || e.Key == Key.P))
        {
            if (!_completed)
            {
                SetPaused(!_scene.Paused);
            }
        }
        else if (e.Key == Key.F1)
        {
            LoadScene();
        }
    }

    private void Button_Continue(object sender, RoutedEventArgs e)
    {
        if (_scene != null)
        {
            if (!_completed)
            {
                SetPaused(false);
            }
        }
    }

    private void Button_Restart(object sender, RoutedEventArgs e)
    {
        LoadScene();
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