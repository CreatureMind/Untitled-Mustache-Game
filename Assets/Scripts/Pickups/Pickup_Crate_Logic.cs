using System;
using UnityEngine;

public class Pickup_Crate_Logic : Pickup_Base
{
    
    protected override void OnTriggerEnter(Collider other)
    {
        // so it won't trigger when the player is inside the crate
    }

    protected override void DoAction()
    {
        var drop = Pickup_Util.RandomizePickup();
        var obj = Pool_Manager.Instance.GetObjectFromPool(drop);
        obj.transform.position = transform.position;
    }
    protected override void ReturnToPool()
    {
        Pool_Manager.Instance.ReturnToPool(gameObject, PoolType.PickupCrate);
    }
}
