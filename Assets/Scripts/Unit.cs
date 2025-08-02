using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] protected Unit_Data unitData;
    public Unit_Data UnitData => unitData;
    [SerializeField] protected Rigidbody _rb;

    protected bool HasCollided = false;

    public Rigidbody Rigidbody => _rb;
    
    protected MovementState _movementState;
    public MovementState MovementState => _movementState;

    private Stat_Handler _statHandler;
    public Stat_Handler StatHandler => _statHandler;

    protected virtual void Awake()
    {
        _statHandler = new Stat_Handler(unitData);
    }

    public void SetMovementState(MovementState state)
    { 
        _movementState = state;
    }

    protected IEnumerator ResetCollisionFlag()
    {
        yield return new WaitForSeconds(0.5f); // Adjust as necessary
        HasCollided = false;
    }
}

public enum MovementState
{
    Idle,
    Moving,
    Attack,
    GotHit
}