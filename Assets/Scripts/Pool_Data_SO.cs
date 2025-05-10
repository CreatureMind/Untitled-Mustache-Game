using UnityEngine;

public class Pool_Data_SO : ScriptableObject
{
    private PoolType _poolType;
    public PoolType PoolType => _poolType;
    private GameObject _prefab;
    public GameObject Prefab => _prefab;
}
