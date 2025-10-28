namespace Atlantis.Game;

/// <summary>
/// Items that you can pickup. This differs from GameControl that assumes you can't pickup an item.
/// </summary>
public class Item : GameControl
{
    public bool IsPickup = true; // Can pickup item
    public float PickupDelay = 0f;
    
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
        PickupDelay = 1.0f;
    }
    
    public override void OnSensorStart(GameShape sensor, GameShape visitor)
    {
        if (visitor.Control is Player player)
        {
            if (PickupDelay <= 0 && IsPickup)
                player.Inventory.PickUp(this);
        }
    }

    public override void OnUpdate(float dt)
    {
        if (PickupDelay > 0)
            PickupDelay -= dt;
    }
}