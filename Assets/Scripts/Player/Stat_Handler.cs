using System;
using UnityEngine;
using UnityEngine.Events;

public class Stat_Handler
{
    private Unit_Data _unitData;
    private float weight;
    private float damage;
    private float knockback;
    private float attackingStateTime;
    private int _currentPercent;

    private int health;

    public static Action GameOver;
    public static Action<int> PlayerTookDamage;
    public static Action<int> EnemyTookDamage;

    public Stat_Handler(Unit_Data unitData)
    {
        _unitData = unitData;
        weight = unitData.Weight;
        damage = unitData.Damage;
        knockback = unitData.Knockback;
        attackingStateTime = unitData.AttackingStateTime;
        _currentPercent = 0;

        health = 3; // Default health value, can be modified as needed
    }
    
    public float Weight => weight;
    public float Damage => damage;
    public float Knockback => knockback;
    public float AttackingStateTime => attackingStateTime;
    public int CurrentPercent => _currentPercent;
    public int Health => health;
        
    public void PlayerDied()
    {
        health -= 1;
        if (health <= 0)
        {
            GameOver?.Invoke();
        }
    }
    
    public void ResetStats()
    {
        weight = _unitData.Weight;
        damage = _unitData.Damage;
        knockback = _unitData.Knockback;
        attackingStateTime = _unitData.AttackingStateTime;
        _currentPercent = 0;

        health = 3; // Reset health to default value
    }
    
    public void SetStats(float weight, float damage, float knockback, float attackingStateTime, int currentPercent)
    {
        this.weight = weight;
        this.damage = damage;
        this.knockback = knockback;
        this.attackingStateTime = attackingStateTime;
        this._currentPercent = currentPercent;
    }

    public void Heal()
    {
        if (health < 3)
        {
            health += 1;
        }
        Debug.Log("Player healed.");
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

        Debug.Log($"{this} Taking Damage: {damage} CurrentPercent: {_currentPercent}");
    }
}
