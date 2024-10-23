using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 단일 프리펩에 대한 풀을 정의한다.
/// </summary>
class Pool
{
    GameObject _prefab;
    IObjectPool<GameObject> _pool;

    Transform _root;
    Transform Root
    {
        get
        {
            if (_root == null)
            {
                var root = new GameObject($"@{_prefab.name}_Root");
                _root = root.transform;
            }

            return _root;
        }
    }

    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        
        // 왼쪽부터 각각 생성, 획득, 반환, 파괴에 대한 델리게이트를 넘겨준다.
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    public GameObject Pop()
    { 
        return _pool.Get();
    }

    public void Push(GameObject poolObject)
    {
        if(poolObject.activeSelf)
            _pool.Release(poolObject);
    }

    #region 오브젝트 풀 함수
    private GameObject OnCreate()
    {
        GameObject instance = GameObject.Instantiate(_prefab);
        instance.transform.SetParent(Root);
        instance.name = _prefab.name;
        return instance;
    }

    private void OnRelease(GameObject poolObject)
    {
        poolObject.SetActive(false);
    }

    private void OnGet(GameObject poolObject)
    {
        try
        {
            poolObject.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void OnDestroy(GameObject poolObject)
    {
        GameObject.Destroy(poolObject);
    }
    #endregion
}

public class PoolManager
{
    Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    public GameObject Pop(GameObject prefab)
    {
        if (!_pools.ContainsKey(prefab.name))
        {
            CreatePool(prefab);
        }

        return _pools[prefab.name].Pop();
    }

    public bool Push(GameObject poolObject)
    {
        if (!_pools.ContainsKey(poolObject.name))
            return false;

        _pools[poolObject.name].Push(poolObject);
        return true;
    }

    private void CreatePool(GameObject prefab)
    {
        Pool pool = new Pool(prefab);
        _pools.Add(prefab.name, pool);
    }

    public void Clear()
    {
        _pools.Clear();
    }
}
