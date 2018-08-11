using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Cacher : MonoBehaviour
{
    #region Variables
    // Inspector-assignable variables
    [SerializeField] private byte[] numberToPool;
    [SerializeField] private GameObject[] prefabsForCloning;        // 0 - Bullet trail. 1 - Explosion. 2 - Chaser. 3 - Roamer
    [SerializeField] private string[] containerNames;

    public static BillBoardManager billBoardManager;                // Not on pauseable list
    public static GameObject character;
    public static GameObject[][] pooledAssets;                      // All instantiated objects are in this container
    public static HUDManager hudManager;                            // Not on pauseable list
    public static List<GameObjectBase> pauseableGameObjects;        // First object/script will always be the player
    public static SpawnManager spawnManager;
    public static Stopwatch gameTime;                               // Not on pauseable list, but paused in the game controller
    public static Transform characterTransform;
    private static uint score;
    #endregion

    #region Initialization
    public void Awake()
    {
        billBoardManager = GameObject.Find("BillBoards").GetComponent<BillBoardManager>();
        character = GameObject.Find("Character");
        pooledAssets = new GameObject[prefabsForCloning.Length][];
        hudManager = GameObject.Find("HUD").GetComponent<HUDManager>();
        pauseableGameObjects = new List<GameObjectBase>();
        spawnManager = GameObject.Find("Spawners").GetComponent<SpawnManager>();
        pauseableGameObjects.Add(character.GetComponent<CharacterManager>());
        pauseableGameObjects.Add(spawnManager);
        characterTransform = character.transform;
        score = 0;
    }
    public void Start()
    {
        gameTime = GameObject.Find("GameController").GetComponent<GameController>().GetGameTime();
        billBoardManager.UpdateBillBoardText(2, score);

        // Pooling all poolable objects
        for (byte prefabIndex = 0; prefabIndex < (byte)prefabsForCloning.Length; ++prefabIndex)
            Pooler.PoolAssets(prefabIndex, numberToPool[prefabIndex], prefabsForCloning[prefabIndex], pooledAssets, containerNames[prefabIndex]);
    }
    #endregion

    #region Public Interface
    public static void AlterScore(uint _value)
    {
        score += _value;
        billBoardManager.UpdateBillBoardText(2, score);
    }
    public static uint GetScore() { return score; }
    public static void PlayerDeath()
    {
        // When player dies, all enemies will get cleared off of the screen
        foreach (GameObjectBase objectScript in pauseableGameObjects)
        {
            // Disable object, if it is an enemy
            if (objectScript.gameObject.tag == "Enemy")
                objectScript.InstaKill();
        }
    }
    #endregion
}