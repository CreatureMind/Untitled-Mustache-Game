using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool_Handler : MonoBehaviour
{
    [SerializeField] private List<Pool> _pools;
    private Dictionary<PoolType , Pool> _poolDictionary;

    void Awake()
    {
        _poolDictionary = new Dictionary<PoolType, Pool>();
        foreach (var pool in _pools)
        {
            _poolDictionary.Add(pool.PoolType, pool);
        }
    }
    
    public GameObject GetObjectFromPool(PoolType poolType)
    {
        if (_poolDictionary.TryGetValue(poolType, out var pool))
        {
            var poolQ = pool.PoolQueue;
            if (poolQ.Count == 0)        
            {
                pool.CreatePool(poolQ.Count * 2);
            }
            var obj = poolQ.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }
    
}

public enum PoolType
{
    Enemy,
    //extend this enum for other types of pools
}

[Serializable]
public class Pool //the basic class container for a pool
{
    private PoolType _poolType;
    public PoolType PoolType => _poolType;
    private Queue<GameObject> _pool;
    public Queue<GameObject> PoolQueue => _pool;
    private GameObject _prefab;
    private ScriptableObject _scriptableObject;
    private int _size;
    
    public Pool(GameObject prefab, ScriptableObject scriptableObject , int size)
    {
        _pool = new Queue<GameObject>();
        _prefab = prefab;
        _scriptableObject = scriptableObject;
        _size = size;
        CreatePool(size);
    }
    
    public void CreatePool(int size) //creates and doubles as pool extender
    {
        for (int i = 0; i < size; i++)
        {
            var obj = GameObject.Instantiate(_prefab);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
        if (_pool.Count != _size)
        {
            _size = _pool.Count;
        }
    }
    
}
