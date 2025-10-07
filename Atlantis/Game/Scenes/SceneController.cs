using System.Windows;

namespace Atlantis.Game;

public class SceneController
{
    private readonly MainWindow _window;
    private Scene? _scene;
    private PauseScene? _pauseScene = null;
    
    public SceneController(MainWindow window)
    {
        _window = window;
        _scene = new GameScene(window); // This can be changed once there is a home screen.
    }
    
    public void Destroy()
    {
        Scene scene = _scene;
        _pauseScene = null; // _pauseScene may already be null, but this makes sure of it.

        switch (_scene)
        {
            case GameScene:
                _scene = new GameScene(_window);
                break;
        }
    }
}