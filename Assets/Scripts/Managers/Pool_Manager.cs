using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool_Manager : MonoBehaviour
{
    private static Pool_Manager instance;
    public static Pool_Manager Instance => instance;
    
    [SerializeField] private List<Pool_Data_SO> _pools;
    private Dictionary<PoolType , Queue<GameObject>> _poolDictionary;
    [SerializeField] private int _size;
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        
        _poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();
        foreach (var poolData in _pools)
        {
            _poolDictionary.Add(poolData.PoolType, new Queue<GameObject>());
            var poolQ = _poolDictionary[poolData.PoolType];
            CreatePool(poolData, poolQ, _size);
        }
    }
    private void CreatePool(Pool_Data_SO poolData, Queue<GameObject> Q, int size) //creates and doubles as pool extender
    {
        for (int i = 0; i < size; i++)
        {
            var obj = Instantiate(poolData.Prefab);
            obj.SetActive(false);
            Q.Enqueue(obj);
        }
    }
    
    public GameObject GetObjectFromPool(PoolType poolType)
    {
        if (_poolDictionary.TryGetValue(poolType, out var poolQ))
        {
            if (poolQ.Count == 0)
            {
                ExtendPool(poolType, poolQ);
            }
            var obj = poolQ.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    private void ExtendPool(PoolType poolType, Queue<GameObject> poolQ)
    {
        for(int i = 0; i < _size; i++)
        {
            var obj = Instantiate(GetPrefabFromPoolDataList(poolType));
            obj.SetActive(false);
            poolQ.Enqueue(obj);
        }
    }

    public void ReturnToPool(GameObject obj, PoolType poolType)
    {
        if (_poolDictionary.TryGetValue(poolType, out var poolQ))
        {
            obj.SetActive(false);
            poolQ.Enqueue(obj);
        }
    }
    
    public void ReturnToPool(GameObject obj)
    {
        foreach (var pool in _pools)
        {
            if (pool.Prefab == obj)
            {
                ReturnToPool(obj, pool.PoolType);
                return;
            }
        }
        Debug.LogError($"Object {obj.name} not found in any pool.");
    }
    
    public GameObject GetPrefabFromPoolDataList(PoolType poolType)
    {
        foreach (var poolData in _pools)
        {
            if (poolData.PoolType == poolType)
            {
                return poolData.Prefab;
            }
        }
        Debug.LogError($"Pool of type {poolType} not found in Pool_Data_SO list.");
        return null;
    }
}

public enum PoolType
{
    Enemy,
    PickupHealth, 
    PickupCrate,
}
