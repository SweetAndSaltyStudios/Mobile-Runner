using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singelton<ObjectPoolManager>
{

    private Dictionary<string, Stack<GameObject>> pooledObjects = new Dictionary<string, Stack<GameObject>>();

    public GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        var prefabName = prefab.name;

        pooledObjects.TryGetValue(prefabName, out Stack<GameObject> pooledInstances);

        if(pooledInstances != null)
        {
            if (pooledInstances.Count > 0) 
            {
                var instance = pooledInstances.Pop();
                instance.transform.SetPositionAndRotation(position, rotation);
                instance.SetActive(true);
                return instance;
            }
            else
            {
                return CreatePrefabInstnace(prefab, position, rotation, parent);
            }
        }
        else
        {
            pooledObjects.Add(prefabName, new Stack<GameObject>());

            return CreatePrefabInstnace(prefab, position, rotation, parent);         
        }
    }

    public void DespawnObject(GameObject prefabInstance)
    {
        prefabInstance.SetActive(false);

        var prefabName = prefabInstance.name;

        pooledObjects.TryGetValue(prefabName, out Stack<GameObject> pooledInstances);

        if (pooledInstances != null)
        {
            pooledInstances.Push(prefabInstance);
        }
        else
        {
            pooledObjects.Add(prefabName, new Stack<GameObject>());
            pooledObjects[prefabName].Push(prefabInstance);
        }
    }

    public void PrecreatePrefabInstances(GameObject prefab, int instanceAmount, Transform parent)
    {
        for (int i = 0; i < instanceAmount; i++)
        {
            var newPrefabInstance = SpawnObject(prefab, Vector3.zero, Quaternion.identity, parent);
            newPrefabInstance.SetActive(false);
        }
    }

    private GameObject CreatePrefabInstnace(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        var newPrefabInstnace = Instantiate(prefab, position, rotation, parent);
        newPrefabInstnace.name = prefab.name;
        return newPrefabInstnace;
    }
}
