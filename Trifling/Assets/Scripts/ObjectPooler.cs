using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler current;
    public GameObject prefab;
    public bool willGrow;
    public int initialPoolAmount = 20;

    private List<GameObject> pooledObjects;

    void Awake() {
        //Called first even if not enabled
        //Used for making references
        //Only called once in lifetime of obj
        current = this;
    }

    // Use this for initialization
    void Start () {
        //Called after the first update but only if enabled
        //Only called once in lifetime of obj
        CreatePool();
	}

    public GameObject GetPooledObject()
    {
        //Return an Inactive pooled GameObject
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];    //Return first inactive gameobject in pool to be used
            }
        }

        if (willGrow)
        {
            //If all objects in pool are being used and pool is allowed to grow create and return a new gameobject
            return ExpandPool();
        }

        return null;    //return null if all gameobjects in pool are being used and not allowed to make more
    }

    private void CreatePool()
    {
        //Create Initial Pool of Prefab objects
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < initialPoolAmount; i++)
        {
            ExpandPool();
        }
    }

    private GameObject ExpandPool()
    {
        //Increase Pool by 1 and return the newly added GameObject
        GameObject obj = (GameObject)Instantiate(prefab);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }
}
