using System;
using UnityEngine;

public abstract class Pickup_Base : MonoBehaviour
{
    [SerializeField] protected PoolType _poolType;

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DoAction();
            ReturnToPool();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DoAction();
            ReturnToPool();
        }
    }

    protected abstract void DoAction();

    protected virtual void ReturnToPool()
    {
        Pool_Manager.Instance.ReturnToPool(gameObject, _poolType);
    }
}
