using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : RecycleObject
{
    public GameObject originalPrefab;

    public int poolSize = 64;

    T[] Pool;

    Queue<T> Queue;

    public void Initialize()
    {
        if (Pool == null)
        {
            Pool = new T[poolSize];
            Queue = new Queue<T>(poolSize);

            GenerateObject(0, poolSize, Pool);
        }
        else 
        {
            foreach (T obj in Pool) 
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    private void GenerateObject(int start, int end, T[] result)
    {
        for (int i = 0; i<end; i++) 
        {
            GameObject obj = Instantiate(originalPrefab, transform);
            obj.name = $"{originalPrefab.name}_{i}";

            T comp = obj.GetComponent<T>();
            comp.onDisable += () => Queue.Enqueue(comp);

            OnGenerateObject(comp);

            result[i] = comp;
            obj.SetActive(false);
        }
    }

    protected virtual void OnGenerateObject(T comp) 
    {
        
    }

    public T GetObject(Vector3? position = null, Vector3? eulerAngle = null)
    {
        if (Queue.Count > 0)
        {
            T comp = Queue.Dequeue();
            comp.transform.position = position.GetValueOrDefault();
            comp.transform.rotation = Quaternion.Euler(eulerAngle.GetValueOrDefault());
            comp.gameObject.SetActive(true);
            OnGetObject(comp);
            return comp;
        }
        else
        {
            ExpendPool();
            return GetObject(position, eulerAngle);
        }
    }

    protected virtual void OnGetObject(T component) { }

    void ExpendPool() 
    {
        Debug.LogWarning($"{gameObject.name} 풀 사이즈 증가 {poolSize*2}");
        int newSize = poolSize * 2;
        T[] newPool = new T[newSize];
        for (int i= 0; i < poolSize; i++) 
        {
            newPool[i] = Pool[i];
        }
        GenerateObject(poolSize, newSize, newPool);

        poolSize = newSize;
        Pool = newPool;
    }
}
