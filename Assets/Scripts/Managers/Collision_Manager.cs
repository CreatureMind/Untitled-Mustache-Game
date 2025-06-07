using System;
using UnityEngine;


public class Collision_Manager : MonoBehaviour
{
    
    //[SerializeField, Range(50,300)] private int sendFlyingThreshold;
    //[SerializeField, Range(1,100)] private float sendFlyingMultiplier;
    //[SerializeField, Range(0,1)] private float normalKnockbackMultiplier;
    [SerializeField] private int DropRange;
    private int bothCollisionCount;
    private OtherType _otherType;
    private static Action<Unit, Unit> OnUnitCollision;
    
    
    void OnEnable()
    {
        //OnUnitCollision += UnitCollision;
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

    /*private void UnitCollision(Unit currentUnit, Unit otherUnit)
    {
        if(otherUnit.MovementState == MovementState.Attack)
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
        
        var kbDirection = (other.transform.position - me.transform.position).normalized;
        if (result >= sendFlyingThreshold - other.CurrentPercent)
        {
            other.Rigidbody.AddForce(kbDirection * result * sendFlyingMultiplier, ForceMode.Impulse);
            Debug.Log("Implementing Bye Bye Logic");
        }
        else
        {
            other.Rigidbody.AddForce(kbDirection * result * normalKnockbackMultiplier, ForceMode.Impulse);
            Debug.Log("Implementing Normal Logic");
        }

        otherUnit.StatHandler.TakeDamage(currentUnit.StatHandler.Damage, _otherType);
    }*/

    
/*    private float CalculateHitResult(Unit currentUnit, Unit otherUnit)
    {
        //extracted so we can change at any moment
        var currentData = currentUnit.StatHandler;
        var otherData = otherUnit.StatHandler;
        
        float result = (Mathf.Log(currentData.))

        return result;
    }*/

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
        //OnUnitCollision -= UnitCollision;
        OnUnitCollision -= PickupDrop;
    }
}

public enum OtherType
{
    Player,
    Enemy
}