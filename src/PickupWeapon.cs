using UnityEngine;
using System.Collections;

public class PickupWeapon : Pickup 
{
    public WeaponManager.WEAPON type;

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
        player.addWeapon(type);
        base.givePickup(player);
    }
}
