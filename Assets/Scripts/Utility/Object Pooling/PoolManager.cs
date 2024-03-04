using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }
    public string poolObjectPath = "Pool Objects";
    
    PoolObject[] poolObjects;
    GameObject poolParent;
    Queue<PoolInstance>[] allPools;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(this);
        }
        poolParent = new GameObject("PoolParent");
        poolParent.transform.SetParent(transform);
        GameObject obj;
        poolObjects = Resources.LoadAll<PoolObject>(poolObjectPath);
        allPools = new Queue<PoolInstance>[poolObjects.Length];
        int id = 0;
        foreach (PoolObject poolObject in poolObjects)
        {
            allPools[id] = new Queue<PoolInstance>();
            for (int i = 0; i < poolObject.instanceCount; i++)
            {
                int variantIndex = UnityEngine.Random.Range(0, poolObject.prefabs.Length);
                obj = Instantiate(poolObject.prefabs[variantIndex], Vector3.one * 1000, Quaternion.identity, poolParent.transform);
                PoolInstance poolInstance = obj.AddComponent<PoolInstance>();
                poolInstance.lifeTime = 10f;
                obj.SetActive(false);
                allPools[id].Enqueue(poolInstance);
            }
            id++;
        }
    }

    PoolInstance objectFromPool;
    /// <summary>
	/// Spawns a pool object with the given objectID at the given position and rotation.
    /// Logs a warning if the objectID is invalid.
	/// </summary>
	/// <returns> The pool object's GameObject. null if the objectID is invalid. </returns>
    public GameObject SpawnObject(string objectID, Vector3 position, Quaternion rotation)
    {
        int poolIndex = Array.FindIndex(poolObjects, pool => pool.name == objectID);
        if (poolIndex < 0 || poolIndex >= poolObjects.Length)
        {
            Debug.LogWarning("Invalid Pool Object ID");
            return null;
        }
        objectFromPool = allPools[poolIndex].Dequeue();
        objectFromPool.lifeTime = -1;
        objectFromPool.transform.position = position;
        objectFromPool.transform.rotation = rotation;
        objectFromPool.gameObject.SetActive(true);
        allPools[poolIndex].Enqueue(objectFromPool);
        return objectFromPool.gameObject;
    }

    /// <summary>
	/// Spawns a pool object with the given objectID at the given position and rotation.
    /// Also sets the object's lifetime to lifeTime.
    /// Logs a warning if the objectID is invalid.
	/// </summary>
	/// <returns> The pool object's GameObject. null if the objectID is invalid. </returns>
    public GameObject SpawnObjectWithLifetime(string objectID, Vector3 position, Quaternion rotation, float lifeTime)
    {
        return SpawnObjectWithLifetime(objectID, position, rotation, Vector3.one, lifeTime);
    }

    /// <summary>
	/// Spawns a pool object with the given objectID with the given position, rotation, and scale.
    /// Logs a warning if the objectID is invalid.
	/// </summary>
	/// <returns> The pool object's GameObject. null if the objectID is invalid. </returns>
    public GameObject SpawnObjectWithLifetime(string objectID, Vector3 position, Quaternion rotation, Vector3 scale, float lifeTime)
    {
        int poolIndex = Array.FindIndex(poolObjects, pool => pool.name == objectID);
        if (poolIndex < 0 || poolIndex >= poolObjects.Length)
        {
            Debug.LogWarning("Invalid Pool Object ID");
            return null;
        }
        objectFromPool = allPools[poolIndex].Dequeue();
        objectFromPool.lifeTime = lifeTime;
        objectFromPool.transform.position = position;
        objectFromPool.transform.rotation = rotation;
        objectFromPool.transform.localScale = scale;
        objectFromPool.gameObject.SetActive(true);
        allPools[poolIndex].Enqueue(objectFromPool);
        return objectFromPool.gameObject;

    }

    /// <summary>
	/// [deprecated] Gets the objectID of a poolObject given it's name.
	/// </summary>
	/// <returns> The PoolObject's integer ID. -1 if not found. </returns>
    public int GetPoolObjectID(string objectName)
    {
        int id = Array.FindIndex(poolObjects, poolObject => poolObject.name == objectName);
		if (id == -1)
        {
			Debug.LogWarning("Pool Object with name: " + name + " does not exist!");
        }
		return id;
    }
}
