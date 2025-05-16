using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] protected Unit_Data unitData;
    public Unit_Data UnitData => unitData;
    [SerializeField] protected Rigidbody _rb;
    public Rigidbody Rigidbody => _rb;
    
    protected MovementState _movementState;
    public MovementState MovementState => _movementState;

    private int _currentPercent;
    public int CurrentPercent => _currentPercent;
    
    public void SetMovementState( MovementState state)
    { 
        _movementState = state;
    }

    public void TakeDamage(int damage)
    {
        _currentPercent += damage;
    }

}