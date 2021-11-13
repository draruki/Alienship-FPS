using UnityEngine;
using System.Collections;

public class PickupAmmo : Pickup 
{
    public int ammo;
    public Weapon.TYPE type;

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
        player.addAmmo(ammo, type);
        base.givePickup(player);
    }
}
