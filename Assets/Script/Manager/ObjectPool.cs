using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : SingletonMono<ObjectPool>
{
    [System.Serializable]
    public class PoolItem
    {
        public string key;
        public GameObject prefab;
        public int initialCount = 10;
    }

    public List<PoolItem> poolItems = new List<PoolItem>();

    private Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();

    protected override void Awake()
    {
        base.Awake();

        foreach (var item in poolItems)
        {
            Queue<GameObject> newPool = new Queue<GameObject>();
            for (int i = 0; i < item.initialCount; i++)
            {
              
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    newPool.Enqueue(obj);
                

            }
            poolDict[item.key] = newPool;
            prefabDict[item.key] = item.prefab;
        }
    }

    public GameObject Get(string key)
    {
        if (!poolDict.ContainsKey(key))
        {
        
            return null;
        }

        GameObject obj;
        if (poolDict[key].Count > 0)
        {
            obj = poolDict[key].Dequeue();
        }
        else
        {
          
                obj = Instantiate(prefabDict[key]);
            

        }


        return obj;
    }

    public void Return(string key, GameObject obj)
    {
        obj.SetActive(false);
        if (!poolDict.ContainsKey(key))
        {

            poolDict[key] = new Queue<GameObject>();
        }
        poolDict[key].Enqueue(obj);
    }
}