using System;
using UnityEngine;

public class Pickup_Crate_Logic : Pickup_Base
{
    protected override void DoAction()
    {
        var drop = Pickup_Util.RandomizePickup();
        var obj = Pool_Manager.Instance.GetObjectFromPool(drop);
        
        obj.transform.position = transform.position;
        var pickup = obj.GetComponent<Pickup_Base>();
        
        if (pickup)
        {
            pickup.RandomDropRange(gameObject);
        }
    }
}
