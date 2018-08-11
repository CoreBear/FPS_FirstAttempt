using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn
{
    public static GameObject ReturnSpawnableAsset(GameObject[] _pool)
    {
        // Search each object in the container
        foreach (GameObject gameObject in _pool)
        {
            // If the object is not active in hierarchy, return it
            if (!gameObject.activeInHierarchy)
                return gameObject;
        }

        // If nothing is found, return nothing
        return null;
    }
}