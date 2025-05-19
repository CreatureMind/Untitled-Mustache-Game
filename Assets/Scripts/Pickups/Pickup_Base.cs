using UnityEngine;

public abstract class Pickup_Base : MonoBehaviour
{
    [SerializeField] protected PoolType _poolType { get; private set; }

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
