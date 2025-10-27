namespace Atlantis.Game;

/// <summary>
/// Items that you can pickup. This differs from GameControl that assumes you can't pickup an item.
/// </summary>
public class Item : GameControl
{
    public bool IsPickup = true; // Can pickup item
    
    /// <summary>
    /// Runs when an item a player pickups and item.
    /// </summary>
    /// <returns>-1 when not implemented else the number of items</returns>
    public virtual void Pickup()
    {
        
    }
    
    /// <summary>
    /// Runs when an item a player drops and item.
    /// </summary>
    /// <returns>-1 when not implemented else the number of items</returns>
    public virtual void Drop()
    {
        
    }
    
    public override void OnSensorStart(GameShape sensor, GameShape visitor)
    {
        if (visitor.Control is Player player)
        {
            player.Inventory.PickUp(this);
        }
    }
}