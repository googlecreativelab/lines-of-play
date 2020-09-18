using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class ObjectPool
{
    public GameObject ObjectToPool;
    public string PoolName;
    public int AmountToPool;
    public bool ShouldExpand = true;

    public ObjectPool() { }

    public ObjectPool(GameObject prefab, string name, int amount, bool expandable = true)
    {
        ObjectToPool = prefab;
        PoolName = name;
        AmountToPool = amount;
        ShouldExpand = expandable;
    }

    public void Initialize()
    {
        if (ObjectToPool != null)
        {
            Id = ObjectToPool.GetInstanceID();
            Initialized = true;
        }
    }

    public bool Initialized { get; private set; }
    public int Id { get; private set; }
    public GameObject ParentPoolObject { get; set; }
    public LinkedList<GameObjectContainer> Items { get; } = new LinkedList<GameObjectContainer>();
    public int Recycles { get; set; }
    public int Created { get; set; }
    public int Expands { get; set; }
}

public interface IPoolable
{
    void Spawn();
    void Despawn();
}

public class GameObjectContainer
{
    public ObjectPool Pool;
    public GameObject Object;
    public List<IPoolable> PoolingEnabledComponents;
    public int Cycles;
    public int TimesSkipped;
    public int TimesSelected;
    public int Despawns;
    public int Spawns;
    public int ObjectId;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    public string RootPoolName = "Pooled Objects";

    public List<ObjectPool> Pools;

    public Dictionary<int, GameObjectContainer> Map { get; } = new Dictionary<int, GameObjectContainer>();

    void Awake()
    {
        Instance = this;
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
    }

    private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
    {
        Debug.Log("Resetting Object Pools");
        Reset();
    }

    private void CreatePools()
    {
        foreach (var item in Pools)
        {
            for (int i = 0; i < item.AmountToPool; i++)
            {
                CreatePooledObject(item);
            }
        }
    }

    public void Reset()
    {
        foreach (var pool in Pools)
        {
            if(pool == null)
                continue;

            foreach (var item in pool.Items)
            {
                if(item == null)
                    continue;

                if (item.Object != null)
                {                    
                    Destroy(item.Object);
                }                 
                               
                item.Object = null;
                item.Pool = null;
                item.PoolingEnabledComponents.Clear();
            }
            pool.Items.Clear();
            pool.ParentPoolObject = null;
        }
        Map.Clear();
        CreatePools();
    }

    /// <summary>
    /// Find/Create the parent which pooled objects should be attached to.
    /// </summary>
    private GameObject GetParentPoolObject(string objectPoolName)
    {
        if (string.IsNullOrEmpty(objectPoolName))
            objectPoolName = RootPoolName;

        var parentObject = GameObject.Find(objectPoolName);
        if (parentObject != null)
            return parentObject;

        parentObject = new GameObject
        {
            name = objectPoolName
        };

        if (objectPoolName == RootPoolName)
            return parentObject;

        var root = GameObject.Find(RootPoolName) ?? GetParentPoolObject(RootPoolName);

        parentObject.transform.parent = root.transform;
        return parentObject;
    }

    /// <summary>
    /// Create a new item for a given pool
    /// </summary>
    private GameObjectContainer CreatePooledObject(ObjectPool pool)
    {
        if (pool.ObjectToPool == null)
        {
            throw new Exception($"Object pool entry '{pool.PoolName}' needs a prefab attached");
        }

        if (!pool.Initialized)
            pool.Initialize();        

        var obj = Instantiate(pool.ObjectToPool);
        obj.name = obj.name;

        if (pool.ParentPoolObject == null)
            pool.ParentPoolObject = GetParentPoolObject(pool.PoolName);       
                
        obj.transform.parent = pool.ParentPoolObject.transform;
        obj.SetActive(false);

        var container = new GameObjectContainer
        {
            Object = obj,
            ObjectId = obj.GetInstanceID(),
            Pool = pool,
            PoolingEnabledComponents = obj.GetComponents<IPoolable>().ToList(),    
        };
        
        Map.Add(obj.GetInstanceID(), container);
        pool.Items.AddFirst(container);
        pool.Created++;
        return container;
    }

    /// <summary>
    /// Create an instance of an GameObject prefab. 
    /// A replacment for 'Instantiate(...)'.
    /// </summary>
    /// <param name="prefab">A game object prefab to create an instance of</param>
    /// <param name="position">The position of the spawned GameObject</param>
    /// <param name="rotation">The rotation of the spawned GameObject</param>
    /// <returns>pooled GameObject</returns>
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var id = prefab.GetInstanceID();
        var pool = GetPoolForPrefab(id);
        if (pool == null)
        {
            pool = new ObjectPool(prefab, prefab.name, 25);
            //Debug.Log($"Dynamically creating pool for prefab {prefab.name}");
            Pools.Add(pool);
            //throw new Exception($"Unable to find object pool for type");
        }

        var container = FindFreePoolItem(pool);
        if (container == null)
        {
            if (!pool.ShouldExpand)
                return null;

            container = CreatePooledObject(pool);
            pool.Expands++;
        }
        else
        {
            pool.Recycles++;
        }

        container.Spawns++;
        RecycleItem(container, position, rotation);
        return container.Object;
    }

    /// <summary>
    /// Return GameObject to the pool. 
    /// A replacement for 'Destroy(...)'. 
    /// </summary>
    /// <param name="o"></param>
    public void Despawn(GameObject o)
    {
        if (o == null)
            return;

        var container = GetContainer(o.GetInstanceID());
        if (container != null)
        {
            container.Despawns++;
            foreach (var c in container.PoolingEnabledComponents)
            {
                c.Despawn();
            }
        }

        o.SetActive(false);
    }

    /// <summary>
    /// Reset transform and call IPoolable.Spawn on components.
    /// </summary>
    private static void RecycleItem(GameObjectContainer container, Vector3 position, Quaternion rotation)
    {
        var t = container.Object.transform;
        t.rotation = rotation;
        t.position = position;
        container.Object.SetActive(true);
        container.Cycles++;

        foreach (var c in container.PoolingEnabledComponents)
        {
            c.Spawn();
        }
    }

    /// <summary>
    /// Get an item from a given pool that is not being used.
    /// </summary>
    private GameObjectContainer FindFreePoolItem(ObjectPool pool)
    {
        for (int i = 0; i < pool.Items.Count; i++)
        {
            var node = pool.Items.First;
            pool.Items.RemoveFirst();
            pool.Items.AddLast(node);

            // Clean out objects that no longer exist (because of scene unload etc)
            var obj = node.Value.Object;
            if (obj == null)
            {
                DestroyContainer(node.Value);
                continue;
            }

            if (!obj.activeInHierarchy)
            {
                node.Value.TimesSelected++;
                return node.Value;
            }

            node.Value.TimesSkipped++;
        }
        return null;
    }

    private void DestroyContainer(GameObjectContainer container)
    {
        container.Pool.Items.Remove(container);
        Map.Remove(container.ObjectId);
    }

    private ObjectPool GetPoolForPrefab(int prefabInstanceId)
    {
        for (int i = 0; i < Pools.Count; i++)
        {
            var pool = Pools[i];
            if (pool.Id == prefabInstanceId)
                return pool;
        }
        return null;
    }

    private GameObjectContainer GetContainer(int gameObjectInstanceId)
    {
        GameObjectContainer container;
        Map.TryGetValue(gameObjectInstanceId, out container);
        return container;
    }

}
