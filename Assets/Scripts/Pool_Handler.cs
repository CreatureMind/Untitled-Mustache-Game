using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool_Handler : MonoBehaviour
{
    [SerializeField] private List<Pool_Data_SO> _pools;
    private Dictionary<PoolType , Queue<GameObject>> _poolDictionary;
    [SerializeField] private int _size;
    
    void Awake()
    {
        _poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();
        foreach (var poolData in _pools)
        {
            _poolDictionary.Add(poolData.PoolType, new Queue<GameObject>());
            var poolQ = _poolDictionary[poolData.PoolType];
            CreatePool(poolData, poolQ, _size);
        }
    }
    public void CreatePool(Pool_Data_SO poolData, Queue<GameObject> Q, int size) //creates and doubles as pool extender
    {
        for (int i = 0; i < size; i++)
        {
            var obj = GameObject.Instantiate(poolData.Prefab);
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
                ExtendPool(poolQ);
            }
            var obj = poolQ.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    private void ExtendPool(Queue<GameObject> poolQ)
    {
        for(int i = 0; i < _size; i++)
        {
            var obj = Instantiate(_tempQ.Peek());
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
    
}

public enum PoolType
{
    Enemy,
    //extend this enum for other types of pools
}
