namespace Atlantis.Game;

public class Inventory
{
    private readonly GameScene _scene;
    
    public Inventory(Player player, GameScene scene)
    {
        _scene = scene;
    }
    
    // All items inherit from GameControl.
    private readonly List<Item> _items = [];

    /// <summary>
    /// Gets all items
    /// </summary>
    /// <returns>_items List</returns>
    public List<Item> GetItems()
    {
        return _items;
    }

    /// <summary>
    /// Removes the item from GameScene and stores the item in _items.
    /// </summary>
    /// <param name="item">Item to be stored</param>
    public void PickUp(Item item)
    {
        _scene.DestroyControl(item);
        _items.Add(item);
    }
    
    /// <summary>
    /// Stores the item in _items.
    /// </summary>
    /// <param name="item">Item to be dropped</param>
    public void DropItem(Item item)
    {
        _items.Remove(item);
    }
}