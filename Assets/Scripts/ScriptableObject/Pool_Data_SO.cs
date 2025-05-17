using UnityEngine;

[CreateAssetMenu(fileName = "Pool", menuName = "Scriptable Objects/Pool_Data")]

public class Pool_Data_SO : ScriptableObject
{
    [SerializeField] private PoolType _poolType;
    
    [SerializeField] private GameObject _prefab;
    
    public PoolType PoolType => _poolType;
    public GameObject Prefab => _prefab;
}