using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
    GameScene? _scene;

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
        Win(1); 
    }

    /// <summary>
    /// Everything that happens when you win a level
    /// </summary>
    /// <param name="level"></param>
    /// <param name="name"></param>
    public void Win(int level)
    {
        int score = Score.Calculation();
        HighscorePage.AddRecord(level, score, _save.Name);
        HigscoreTable.ItemsSource = HighscorePage.ReadData(level);

        int score = Score.Calculation(_scene.Time);
        HighscorePage.AddRecord(_level, score, _save.Name);
        _window.PushPage(new HighscorePage(_window, _save, _level));
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
        RootGrid.Children.Add(_canvas);
    }
}