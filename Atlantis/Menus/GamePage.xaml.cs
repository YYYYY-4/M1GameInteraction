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
    Page _page;
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
    }


    /// <summary>
    /// Everything that happens when you win a level
    /// </summary>
    public void Win()
    {
        int score = Score.Calculation(_scene.Time);
        HighscorePage.AddRecord(_level, score, _save.Name);
        _window.PushPage(new HighscorePage(_window, _save, _level));
    }
    
    /// <typeparam name="T">Scene which inherits Page and defines a Canvas at it's root.</typeparam>
    public void LoadScene<T>() where T : Page
    {
        if (_scene != null)
            _scene.Destroy();
        
        _page = new DemoLevel();
        _canvas = (Canvas) _page.Content;
        _scene = new GameScene(_window, this, _canvas);
        
        _page.Content = null;
        Content = _canvas;
    }
}