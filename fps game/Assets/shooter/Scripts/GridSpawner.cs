using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class GridSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private int numObjLong, numObjHigh, numObjWide;
    [SerializeField] private Vector3 objSpacing;
    [SerializeField] private bool spawnObjects = false;

    private void Update()
    {
        if (spawnObjects == true)
        {
            spawnObjects = false;
            Delete();
            Spawn();
        }
    }

    private void Delete()
    {
        foreach (Transform childTR in this.transform)
            DestroyImmediate(childTR.gameObject);
    }

    private void Spawn()
    {
        for (int z = 0; z < numObjWide; z++)
        {
            for (int y = 0; y < numObjHigh; y++)
            {
                for (int x = 0; x < numObjLong; x++)
                {
                    Instantiate(objectToSpawn, objectToSpawn.transform.position + new Vector3(objSpacing.x * x, objSpacing.y * y, objSpacing.z * z),//objSpacing*x, 
                        Quaternion.identity, this.transform);
                }
            }
        }
    }
}
