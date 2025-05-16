using System;
using UnityEngine;


public class Collision_Manager : MonoBehaviour
{
    private static Action<Unit, Unit> OnUnitCollision;
    [SerializeField] private int sendFlyingThreshold;
    [SerializeField, Range(1,100)] private float sendFlyingMultiplier;
    [SerializeField, Range(0,1)] private float normalKnockbackMultiplier;
    private int bothCollisionCount;
    
    void OnEnable()
    {
        OnUnitCollision += UnitCollision;
    }

    private void UnitCollision(Unit me, Unit other)
    {
        if(other.MovementState == MovementState.Attack)
        {
            BothAttackStateCollision(me, other);
            return;
        }
        bothCollisionCount = 0;
        
        if (other.CompareTag("Player"))
        {
            Player_Manager.Instance.MovementHandler.SetMovementState( MovementState.GotHit);
        }
        
        var result = CalculateHitResult(me, other);
        
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
        }
        other.TakeDamage(me.UnitData.Damage);
    }

    
    private float CalculateHitResult(Unit me, Unit other)
    {
        //extracted so we can change at any moment
        var meData = me.UnitData;
        var otherData = other.UnitData;
        var otherPercent = other.CurrentPercent;
        
        var meSpeed = new Vector3(me.Rigidbody.linearVelocity.x, 0, me.Rigidbody.linearVelocity.z);
        float result = meData.Weight - otherData.Weight + meData.Damage + meSpeed.magnitude;
        return result;
    }

    private void BothAttackStateCollision(Unit me, Unit other)
    {
        bothCollisionCount++;
        if(bothCollisionCount == 2)
        {
            
        }
    }
    public static void InvokeUnitCollision(Unit me, Unit other)
    {
        OnUnitCollision?.Invoke(me, other);
    }
    
    


}

    
