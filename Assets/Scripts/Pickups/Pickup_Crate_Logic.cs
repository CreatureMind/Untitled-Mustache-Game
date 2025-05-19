using System;
using UnityEngine;

public class Pickup_Crate_Logic : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if  (other.gameObject.CompareTag("Player"))  
        {
            Debug.Log("Pickup Collision");
            var drop = Pickup_Util.RandomizePickup();
            Pool_Manager.Instance.ReturnToPool(gameObject, PoolType.PickupCrate);
            var obj = Pool_Manager.Instance.GetObjectFromPool(drop);
            obj.transform.position = transform.position;
        }
    }
}
