using System;
using UnityEngine;

public class Pickup_Crate_Logic : Pickup_Base
{
    protected override void DoAction()
    {
        var drop = Pickup_Util.RandomizePickup();
        var obj = Pool_Manager.Instance.GetObjectFromPool(drop);
        
        obj.transform.position = transform.position;
        var pickup = obj.GetComponent<Pickup_Heal>();
        
        if (pickup != null)
        {
            pickup.RandomDropRange(gameObject);
        }
    }

    public new void RandomDropRange(GameObject currentObj)
    {
        //base.RandomDropRange(currentObj);
    }
}
