namespace Atlantis.Game;

/// <summary>
/// Items that you can pickup. This differs from GameControl that assumes you can't pickup an item.
/// </summary>
public class Item : GameControl
{
    /// <summary>
    /// Runs when an item a player pickups and item.
    /// </summary>
    /// <returns>-1 when not implemented else the number of items</returns>
    public virtual int Pickup()
    {
        return -1;
    }
    
    /// <summary>
    /// Runs when an item a player pickups and item.
    /// </summary>
    /// <returns>-1 when not implemented else the number of items</returns>
    public virtual int Drop()
    {
        return -1;
    }
}