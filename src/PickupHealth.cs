using UnityEngine;
using System.Collections;

public class PickupHealth : Pickup 
{
    public int health;

	public override void Start () 
    {
        base.Start();
	}
	
    public override void Update () 
    {
        base.Update();
	}

    protected override void givePickup(PlayerEntity player)
    {
        player.addHealth(health);
        base.givePickup(player);
    }
}
