using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public interface iPoolable 
{
    void OnPool();
    void OnUnpool();
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance = null;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this);

            StartCoroutine(InstantiateObjects(() =>
            {
                Debug.Log($"Load of { myPooledObjects.Count } completed successfully");
            }));
        }
    }

    public GameObject[] objectsToPool;
    public int instancesPerObject;
    public int objectsPerFrame;

    private Dictionary<int, List<GameObject>> myPooledObjects = new Dictionary<int, List<GameObject>>();
    private List<GameObject> objectsOutOfPool = new List<GameObject>();

    private IEnumerator InstantiateObjects(Action onComplete = null)
    {
        int aux = 0;

        foreach (GameObject obj in objectsToPool)
        {
            myPooledObjects.Add(obj.GetInstanceID() , new List<GameObject>());

            for (int i = 0; i < instancesPerObject; i++)
            {
                if (aux >= objectsPerFrame)
                {
                    aux = 0;
                    yield return null;
                }

                GameObject newObj = Instantiate(obj, transform);

                newObj.SetActive(false);
                newObj.name.Replace("(clone)" , "");
                myPooledObjects[obj.GetInstanceID()].Add(newObj);
            }
        }

        if (onComplete != null)
            onComplete.Invoke();
    }

    public GameObject GetObjectOfType(int key , Vector3 position = default(Vector3) , Vector3 rotation = default(Vector3) , Transform parent = null)
    {
        if (!myPooledObjects.ContainsKey(key))
        {
            Debug.LogError($"The key {key} is not present in the ObjectPool");
            return null;
        }

        if (myPooledObjects[key].Count <= 0)
        {
            Debug.LogError($"The list with key: {key} has no objects instantiated");
            return null;
        }

        List<GameObject> myList = myPooledObjects[key];

        GameObject pooledObject = myList.First<GameObject>();
        myList.Remove(pooledObject);

        pooledObject.transform.SetParent(parent);

        if (position != null)
            pooledObject.transform.position = position;
        if (rotation != null)
            pooledObject.transform.rotation = Quaternion.Euler(rotation);

        pooledObject.SetActive(true);
        objectsOutOfPool.Add(pooledObject);

        foreach (iPoolable script in pooledObject.GetComponents<iPoolable>())
        {
            script.OnUnpool();
        }

        return pooledObject;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (!obj)
        {
            Debug.LogError("The object intended to be pooled is null");
            return;
        }
        if (!objectsOutOfPool.Contains(obj))
        {
            Debug.LogError($"The object {obj} was not created by the pool, yet there was a request to return it");
            return;
        }

        objectsOutOfPool.Remove(obj);

        myPooledObjects[obj.GetInstanceID()].Add(obj);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;

        foreach (iPoolable script in obj.GetComponents<iPoolable>())
        {
            script.OnPool();
        }
    }
}