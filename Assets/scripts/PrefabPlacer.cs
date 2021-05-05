using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPlacer : MonoBehaviour
{

    public GameObject prefab;
    [HideInInspector]
    public float yOffset = 0f;
    [HideInInspector]
    public float radius = 5f;
    [HideInInspector]
    public bool randomiseYRotation;
    [HideInInspector]
    public bool useGeometryInsteadOfPivot;

    [HideInInspector]
    public int amountOfGO = 1;
    [HideInInspector]
    public List<GameObject> spawnedObjects;


    private void Reset()
    {
        spawnedObjects = new List<GameObject>();
    }
}