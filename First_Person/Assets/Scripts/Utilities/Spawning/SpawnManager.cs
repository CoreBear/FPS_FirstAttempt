using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : GameObjectBase
{
    #region Variables
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private GameObject[] spawners;

    private byte[] arenaBounds;
    private GameObject enemy;
    private Vector3 randomSpawnLocation;
    #endregion

    #region Initialization
    // Do better
    private void Awake()
    {
        Vector3 groundSize = GameObject.Find("Ground").transform.lossyScale;
        arenaBounds = new byte[2] { (byte)(groundSize.x * 5), (byte)(groundSize.z * 5) };
        Debug.Log(arenaBounds[0]);
        randomSpawnLocation.y = 3;
        InvokeRepeating("Spawnstuff", 0, timeBetweenSpawns);
    }
    #endregion

    #region Main Update
    public override void GameUpdate() { return; }
    #endregion

    #region Blackbox
    //private void Spawnstuff()
    //{
    //    // Done lazily. I'm tired.
    //    enemy = Spawn.ReturnSpawnableAsset(Cacher.pooledAssets[3]);
    //    enemy.transform.position = spawners[Random.Range(0, 4)].transform.position;
    //    Cacher.pauseableGameObjects.Add(enemy.GetComponent<GameObjectBase>());
    //    enemy.SetActive(true);
    //}
    private void Spawnstuff()
    {
        // Done lazily. I'm tired.
        enemy = Spawn.ReturnSpawnableAsset(Cacher.pooledAssets[2]);
        randomSpawnLocation.x = Random.Range(-arenaBounds[0], arenaBounds[0]);
        randomSpawnLocation.z = Random.Range(-arenaBounds[1], arenaBounds[1]);
        enemy.transform.position = randomSpawnLocation;
        Cacher.pauseableGameObjects.Add(enemy.GetComponent<GameObjectBase>());
        enemy.SetActive(true);
    }
    #endregion


}