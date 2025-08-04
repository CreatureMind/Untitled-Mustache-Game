using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Level_Data")]
public class Level_Data_SO : ScriptableObject
{
    [SerializeField] private Material levelMaterial;
    [SerializeField] private int normalDifficultyEnemyAmount;
    [SerializeField] private float spawnRadius;
    
    public Material LevelMaterial => levelMaterial;
    public int NormalDifficultyEnemyAmount => normalDifficultyEnemyAmount;
    public float SpawnRadius => spawnRadius;
}