using Atlantis.Game;
using Atlantis.Scene;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Atlantis.Game;
using Atlantis.Scene;

namespace Atlantis.Menus;

/// <summary>
/// This page shows the GameScenes in the Scene folder.
/// </summary>
public partial class GamePage : Page, INotifyPropertyChanged
{
    MainWindow _window;
    Canvas _canvas;
    GameScene _scene;

    public GameScore Score = new GameScore();

    private PlayerSave _save;
    private int _level;
    private bool _completed = false;

    public int LevelIndex => _level;

    public string Level => $"Level {_level + 1}";
    public string LevelScore 
    { 
        get; 

        set; 
    }

    public static readonly Dictionary<int, Type> Levels = new()
    {
        { 0, typeof(DemoLevel) },
        { 1, typeof(DemoLevel2) }
    };

    public event PropertyChangedEventHandler? PropertyChanged;

    public GamePage(MainWindow window, PlayerSave save, int level)
    {
        InitializeComponent();
        DataContext = this;

        _window = window;
        _save = save;
        _level = level;

        LoadScene();

        Focusable = true;
        Focus();

        UpdatePauseVisible();

        Loaded += GamePage_Loaded;
    }

    private void GamePage_Loaded(object sender, RoutedEventArgs e)
    {
        if (_level > 0)
        {
            _window.BoidSimulation.UnmountPanel();
        }
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

        LevelScore = "Score: " + Convert.ToString(score);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LevelScore)));

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
        Panel.SetZIndex(_canvas, -100);

        // use entire grid
        Grid.SetColumnSpan(_canvas, int.MaxValue);
        Grid.SetRowSpan(_canvas, int.MaxValue);

        RootGrid.Children.Add(_canvas);
        LevelStatus.Content = "Paused";
        ContinueButton.Content = "Continue";
        HigscoreTable.ItemsSource = Highscores.ReadData(_level);
        LevelScore = string.Empty;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LevelScore)));

        UpdatePauseVisible();
    }

    // Called by GameScene update to update HUD
    public void GameUpdate(float dt)
    {
        float minutes = float.Floor(_scene.Time / 60.0f);
        float seconds = _scene.Time - minutes * 60.0f;

        _lblScore.Content = $"Score: {Score.Calculation(_scene.Time)}";
        _lblTime.Content = $"Time: {minutes:00}:{seconds:00}";

        var currentItem = _scene.Controls.OfType<Player>().FirstOrDefault()?.Inventory?.GetItem();
        ImageSource? imgResource = null;
        string itemName = string.Empty;

        if (currentItem != null)
        {
            imgResource = currentItem.GetIconResource();
            itemName = currentItem.GetDisplayName();
        }

        if (imgResource != null)
        {
            _imgItem.Source = imgResource;
            _lblItem.Content = $"(G) {itemName}";
            _spItem.Visibility = Visibility.Visible;
        }
        else
        {
            _spItem.Visibility = Visibility.Collapsed;
        }
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

            if (_completed)
            {
                if (Levels.ContainsKey(_level + 1))
                {
                    _level++;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Level)));
                    
                    LoadScene();
                }
                else
                {
                    _window.GoBack();
                }
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