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
    
    void OnDisable()
    {
        OnUnitCollision -= UnitCollision;
        OnUnitCollision -= PickupDrop;
    }

    private void PickupDrop(Unit currentUnit, Unit otherUnit)
    {
        if (currentUnit.CompareTag("Player"))
        {
            if (Pickup_Util.RandomizeCrate())
            {
                SpawnCrate(otherUnit.transform.position);
            }
        }
    }

    private void SpawnCrate(Vector3 spawnPosition)
    {
        Debug.Log("Spawning crate at: " + spawnPosition);

        var obj = Pool_Manager.Instance.GetObjectFromPool(PoolType.PickupCrate);
        obj.transform.position = spawnPosition;

        var pickup = obj.GetComponent<Pickup_Base>();
        pickup?.RandomDropRange(obj);
    }

    private void UnitCollision(Unit currentUnit, Unit otherUnit)
    {
        /*if (otherUnit.MovementState == MovementState.Attack)
        {
            BothAttackStateCollision(currentUnit, otherUnit);
            return;
        }
        bothCollisionCount = 0;*/

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
        
        Vector3 knockbackDirection = (otherUnit.transform.position - currentUnit.transform.position).normalized;
        otherUnit.Rigidbody.AddForce(knockbackDirection * result, ForceMode.Impulse);

        /*// Apply reactive force to the attacker if needed
        if (otherUnit.CompareTag("Player"))
        {
            currentUnit.Rigidbody.linearVelocity = Vector3.zero;
            currentUnit.Rigidbody.AddForce(-knockbackDirection * result / 4f, ForceMode.Impulse);
        }*/

        // Add torque based on the hit direction
        Vector3 contactVector = currentUnit.transform.position - otherUnit.transform.position;
        contactVector.y = 0;

        if (contactVector.sqrMagnitude > 0.001f)
        {
            // Determine if the hit came from left or right relative to forward
            Vector3 forward = otherUnit.transform.forward;
            float directionSign = Mathf.Sign(Vector3.Dot(Vector3.Cross(forward, contactVector), Vector3.up));
            
            // Compute torque strength based on impact velocity
            float impactVelocity = (currentUnit.Rigidbody.linearVelocity - otherUnit.Rigidbody.linearVelocity).magnitude;
            float torqueStrength = directionSign * impactVelocity * 0.5f;

            // Now apply torque in +Y or -Y only
            currentUnit.Rigidbody.AddTorque(Vector3.up * torqueStrength, ForceMode.Impulse);
            otherUnit.Rigidbody.AddTorque(Vector3.up * torqueStrength, ForceMode.Impulse);
        }

        // Damage
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
        Debug.Log("Hit force: " + result);

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
}

public enum OtherType
{
    Player,
    Enemy
}