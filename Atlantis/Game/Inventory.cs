using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Media.Imaging;
using Atlantis.Box2dNet;
using Box2dNet.Interop;

namespace Atlantis.Game;

public class Inventory
{
    private readonly GameScene _scene;
    
    public Inventory(Player player, GameScene scene)
    {
        _scene = scene;
    }
    
    // All items inherit from GameControl.
    private Item? _item = null;

    /// <summary>
    /// Gets the item
    /// </summary>
    /// <returns>_item or null</returns>
    public Item? GetItem()
    {
        return _item;
    }

    /// <summary>
    /// Sets _item based on the item parameter.
    /// </summary>
    /// <param name="item">Item to be stored</param>
    public void PickUp(Item item)
    {
        _scene.DestroyControl(item);
        _item = item;
    }
    
    /// <summary>
    /// Sets _item to null.
    /// </summary>
    public void DropItem(Player player)
    {
        if (_item == null) return;
        Debug.WriteLine("Dropped item!");
        
        Vector2 position = player.Body.GetPosition();
        _scene.ProcessGameControl(_item!, new b2Transform(position, b2Rot.Zero));
        _item.Drop();
        
        _item = null;
    }
}