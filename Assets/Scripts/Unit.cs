using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [SerializeField] protected Unit_Data unitData;
    public Unit_Data UnitData => unitData;
    [SerializeField] protected Rigidbody _rb;

    public Rigidbody Rigidbody => _rb;
    
    protected MovementState _movementState;
    public MovementState MovementState => _movementState;

    private Stat_Handler _statHandler;
    public Stat_Handler StatHandler => _statHandler;

    private void Awake()
    {
        _statHandler = new Stat_Handler(unitData);
    }

    public void SetMovementState( MovementState state)
    { 
        _movementState = state;
    }
    
    public void CreateStatHandler()
    {
        _statHandler = new Stat_Handler(unitData);
    }
}