using Atlantis.Scene;

namespace Atlantis.Game;

public class PauseScene : Scene
{
    private readonly MainWindow _window;
        
    public PauseScene(MainWindow window)
    {
        _window = window;
        _window.LoadScene<PausePage>();
    }

    public override void Destroy()
    {
        
    }
    
    
}