using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [SerializeField] protected Unit_Data unitData;
    public Unit_Data UnitData => unitData;
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] [CanBeNull] private Image percentImage;

    public Rigidbody Rigidbody => _rb;
    
    protected MovementState _movementState;
    public MovementState MovementState => _movementState;

    private int _currentPercent;
    public int CurrentPercent => _currentPercent;

    public static Func<int, Color> EnemyTookDamage;
    public static UnityEvent<int> PlayerTookDamage = new();
    
    public void SetMovementState( MovementState state)
    { 
        _movementState = state;
    }

    public void TakeDamage(int damage)
    {
        _currentPercent += damage;
        if (CompareTag("Player"))
        {
            PlayerTookDamage?.Invoke(_currentPercent);
        }
        else if (percentImage != null)
        {
            percentImage.color = EnemyTookDamage?.Invoke(_currentPercent) ?? Color.white;
        }
        
        Debug.Log($"{this} Taking Damage: {damage} CurrentPercent: {_currentPercent}");
    }

}