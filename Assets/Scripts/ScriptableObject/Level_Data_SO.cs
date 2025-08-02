using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Level_Data")]
public class Level_Data_SO : ScriptableObject
{
    [SerializeField] private Material levelMaterial;
    public Material LevelMaterial => levelMaterial;
    [SerializeField] private int normalDifficultyEnemyAmount;
    public int NormalDifficultyEnemyAmount => normalDifficultyEnemyAmount;
    [SerializeField] private float spawnRadius;
    public float SpawnRadius => spawnRadius;
}