using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler
{ 
    public static void PoolAssets(byte _index, byte _numberOfAssetsToBePooled, GameObject _assetToBeCloned, GameObject[][] _poolContainer, string _containerName)
    {
        GameObject assetClone = null;
        GameObject newContainer = new GameObject() { name = _containerName };
        newContainer.transform.parent = GameObject.Find("PooledObjectsContainer").transform;
        _poolContainer[_index] = new GameObject[_numberOfAssetsToBePooled];

        for (byte currentAssetInstance = 0; currentAssetInstance < _numberOfAssetsToBePooled; ++currentAssetInstance)
        {
            assetClone = Object.Instantiate(_assetToBeCloned);
            assetClone.SetActive(false);
            assetClone.transform.parent = newContainer.transform;
            _poolContainer[_index][currentAssetInstance] = assetClone;
        }
    }
}
