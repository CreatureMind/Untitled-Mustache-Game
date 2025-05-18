using System;
using UnityEngine;

public class Stat_Handler 
{
    private Unit_Data _unitData;
    private float weight;
    private float damage;
    private float knockback;
    private float attackingStateTime;
    
    private 
    
    private int health;

    public static Action GameOver;
    
    public Stat_Handler(Unit_Data unitData)
    {
        _unitData = unitData;
        weight = unitData.Weight;
        damage = unitData.Damage;
        knockback = unitData.Knockback;
        attackingStateTime = unitData.AttackingStateTime;
        
        health = 3; // Default health value, can be modified as needed
    }
    
    public float Weight => weight;
    public float Damage => damage;
    public float Knockback => knockback;
    public float AttackingStateTime => attackingStateTime;
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
        
        health = 3; // Reset health to default value
    }
    
    public void SetStats(float weight, float damage, float knockback, float attackingStateTime)
    {
        this.weight = weight;
        this.damage = damage;
        this.knockback = knockback;
        this.attackingStateTime = attackingStateTime;
    }

    public void Heal()
    {
        if (health < 3)
        {
            health += 1;
        }
    }

}
