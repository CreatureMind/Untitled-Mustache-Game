using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] protected Unit_Data unitData;
    public Unit_Data UnitData => unitData;
    [SerializeField] protected Rigidbody _rb;
    public Rigidbody Rigidbody => _rb;
    
}