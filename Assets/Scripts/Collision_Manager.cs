using System;
using UnityEngine;


public class Collision_Manager : MonoBehaviour
{
    private static Action<Unit> OnPlayerAttack;
    private static Action<Unit> OnEnemyAttack;

    
    public static void InvokePlayerAttack(Unit other)
    {
        OnPlayerAttack?.Invoke(other);
    }
    public static void InvokeEnemyAttack(Unit other)
    {
        OnEnemyAttack?.Invoke(other);
    }
    
}

    
