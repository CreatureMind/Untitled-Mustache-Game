using System;
using UnityEngine;
using UnityEngine.Events;

public class Stat_Handler
{
    private Unit_Data _unitData;
    private float _weight;
    private int _damage;
    private float _knockback;
    private float _attackingStateTime;
    private int _currentPercent;
    private float _currentMagnitude;
    private int _health;

    public static Action GameOver;
    public static Action PlayerDiedButNotGameOver;
    public static Action<int> PlayerTookDamage;
    public static Action<int> EnemyTookDamage;

    public Stat_Handler(Unit_Data unitData)
    {
        _unitData = unitData;
        _weight = unitData.Weight;
        _damage = unitData.Damage;
        _knockback = unitData.Knockback;
        _attackingStateTime = unitData.AttackingStateTime;
        _currentPercent = 0;
        _currentMagnitude = 0;
        _health = 3; // Default health value, can be modified as needed
    }
    public void PlayerDied()
    {
        _health -= 1;
        PlayerDiedButNotGameOver?.Invoke();
        if (_health <= 0)
        {
            Level_Manager.Instance.InvokeOnGameOver();
        }
    }
    
    public void ResetStats(int health = -1)
    {
        _weight = _unitData.Weight;
        _damage = _unitData.Damage;
        _knockback = _unitData.Knockback;
        _attackingStateTime = _unitData.AttackingStateTime;
        _currentPercent = 0;
        if (health == -1)
        {
            _health = 3;
        }
    }
    
    public void SetStats(float weight, int damage, float knockback, float attackingStateTime, int percent)
    {
        _weight = weight;
        _damage = damage;
        _knockback = knockback;
        _attackingStateTime = attackingStateTime;
        _currentPercent = percent;
    }

    public void SetMagnitude(float magnitude)
    {
        _currentMagnitude = magnitude;
        
        if (magnitude <= 0)
            _currentMagnitude = 0;
    }

    public void Heal()
    {
        if (_health < 3)
        {
            _health += 1;
        }
#if UNITY_EDITOR
        Debug.Log("Player healed.");
#endif
    }

    public void TakeDamage(int damage, OtherType otherType)
    {
        _currentPercent += damage;
        if (otherType == OtherType.Player)
        {
            PlayerTookDamage?.Invoke(_currentPercent);
        }
        else
        {
            EnemyTookDamage?.Invoke(_currentPercent);
        }
        
#if UNITY_EDITOR
        Debug.Log($"{otherType} Taking Damage: {damage} CurrentPercent: {_currentPercent}");
#endif
        
    }
    
    public float Weight => _weight;
    public int Damage => _damage;
    public float Knockback => _knockback;
    public float AttackingStateTime => _attackingStateTime;
    public int CurrentPercent => _currentPercent;
    public float CurrentMagnitude => _currentMagnitude;
    public int Health => _health;
        
    
}
