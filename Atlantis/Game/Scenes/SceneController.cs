using System.Windows.Input;

namespace Atlantis.Game;

public class SceneController
{
    private readonly MainWindow _window;
    private Scene? _scene;
    
    public SceneController(MainWindow window)
    {
        _window = window;
        _scene = new GameScene(window); // This can be changed once there is a home screen.
        
        _window.KeyDown += KeyDown;
    }
    
    public void Reload()
    {
        switch (_scene)
        {
            case GameScene:
                _scene = new GameScene(_window);
                break;
            case PauseScene:
                _scene = new PauseScene(_window);
                break;
        }
    }

    public void KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                // _scene!.Destroy();
                _scene = new PauseScene(_window);
                break;
        }
    }
}