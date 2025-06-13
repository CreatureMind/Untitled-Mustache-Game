using System;
using UnityEngine;


public class Collision_Manager : MonoBehaviour
{
    [SerializeField] private int DropRange;
    private int bothCollisionCount;
    private OtherType _otherType;

    private static Action<Unit, Unit> OnUnitCollision;
    
    
    void OnEnable()
    {
        OnUnitCollision += UnitCollision;
        OnUnitCollision += PickupDrop;
    }

    private void PickupDrop(Unit currentUnit, Unit otherUnit)
    {
        if (currentUnit.CompareTag("Player"))
        {
            if (Pickup_Util.RandomizeCrate())
            {
                var obj = Pool_Manager.Instance.GetObjectFromPool(PoolType.PickupCrate);
                Vector2 randomPoint = UnityEngine.Random.insideUnitCircle;
                randomPoint *= DropRange;
                obj.transform.position = new Vector3(randomPoint.x, 0, randomPoint.y) + currentUnit.transform.position;
            }
        }
    }

    private void UnitCollision(Unit currentUnit, Unit otherUnit)
    {
        if (otherUnit.MovementState == MovementState.Attack)
        {
            BothAttackStateCollision(currentUnit, otherUnit);
            return;
        }
        bothCollisionCount = 0;

        if (otherUnit.CompareTag("Player"))
        {
            Player_Manager.Instance.MovementHandler.SetMovementState(MovementState.GotHit);
            _otherType = OtherType.Player;
        }
        else
        {
            _otherType = OtherType.Enemy;
        }

        var result = CalculateHitResult(currentUnit, otherUnit);

        //if sweet spot
        //add buffer

        var knockbackDirection = (otherUnit.transform.position - currentUnit.transform.position).normalized;

        otherUnit.Rigidbody.AddForce(knockbackDirection * result, ForceMode.Impulse);
        Debug.Log("Implementing Bye Bye Logic");


        otherUnit.StatHandler.TakeDamage(currentUnit.StatHandler.Damage, _otherType);
    }


    private float CalculateHitResult(Unit currentUnit, Unit otherUnit)
    {
        //extracted so we can change at any moment
        var currentData = currentUnit.StatHandler;
        var otherData = otherUnit.StatHandler;

        float percent = Mathf.Max(otherData.CurrentPercent, 0f); // just in case
        float logComponent = Mathf.Log10(1f + percent) + 2f;
        float result = (logComponent * logComponent * currentData.CurrentMagnitude * currentData.CurrentMagnitude) / otherData.Weight;
        Debug.Log(result);

        return result;
    }

    private void BothAttackStateCollision(Unit currentUnit, Unit otherUnit)
    {
        bothCollisionCount++;
        if(bothCollisionCount == 2)
        {
            Debug.Log("Both Attack State Collision");
        }
    }

    public static void InvokeUnitCollision(Unit currentUnit, Unit otherUnit)
    {
        OnUnitCollision?.Invoke(currentUnit, otherUnit);
    }

    void OnDisable()
    {
        OnUnitCollision -= UnitCollision;
        OnUnitCollision -= PickupDrop;
    }
}

public enum OtherType
{
    Player,
    Enemy
}