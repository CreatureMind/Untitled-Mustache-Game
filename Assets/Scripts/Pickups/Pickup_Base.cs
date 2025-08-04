using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public abstract class Pickup_Base : MonoBehaviour
{
    [SerializeField] protected PoolType _poolType;

    public static Action ReturnAllPickups;

    void OnEnable()
    {
        ReturnAllPickups += ReturnToPool;
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && gameObject.CompareTag("Crate"))
        {
            // Small delay before spawning drop and destroying crate
            StartCoroutine(DelayedAction());
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            DoAction();
            ReturnToPool();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameObject.CompareTag("Crate"))
        {
            // Small delay before spawning drop and destroying crate
            StartCoroutine(DelayedAction());
        }
        else if (other.CompareTag("Player"))
        {
            DoAction();
            ReturnToPool();
        }
    }

    protected abstract void DoAction();

    private void ReturnToPool()
    {
        Pool_Manager.Instance.ReturnToPool(gameObject, _poolType);
    }

    protected internal virtual void RandomDropRange(GameObject currentObj)
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

    protected virtual void OnDisable()
    {
        ReturnAllPickups -= ReturnToPool;
    }
}