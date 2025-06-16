using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public abstract class Pickup_Base : MonoBehaviour
{
    [SerializeField] protected PoolType _poolType;

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Small delay before spawning drop and destroying crate
            StartCoroutine(DelayedAction());
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Small delay before spawning drop and destroying crate
            StartCoroutine(DelayedAction());
        }
    }

    protected abstract void DoAction();

    protected virtual void ReturnToPool()
    {
        Pool_Manager.Instance.ReturnToPool(gameObject, _poolType);
    }

    protected virtual void RandomDropRange(Unit currentObj)
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * 5f; // Adjust multiplier for drop range
        Vector3 endPos = new Vector3(randomPoint.x, 0, randomPoint.y) + currentObj.transform.position;
    
        transform.DOJump(endPos, jumpPower: 3f, numJumps: 1, duration: 1f).SetEase(Ease.OutQuad);
    }
    
    private IEnumerator DelayedAction()
    {
        // Wait a tiny bit for the knockback to be visible
        yield return new WaitForSeconds(0.2f);
        
        DoAction();
        ReturnToPool();
    }

}
