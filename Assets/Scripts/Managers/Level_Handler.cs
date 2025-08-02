using System;
using System.Collections.Generic;
using UnityEngine;

public class Level_Handler : MonoBehaviour
{
    [SerializeField] private Level_Data_SO levelData;
    public Level_Data_SO LevelData => levelData;
    

    public static Action<Collision> OnCollisionAction;

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionAction?.Invoke(collision);
    }
}