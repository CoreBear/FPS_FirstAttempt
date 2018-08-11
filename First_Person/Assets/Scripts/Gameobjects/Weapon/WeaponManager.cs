using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : GameObjectBase
{



    // 2s for autorifles
    // 1s for shotguns







    #region Variables
    // Inspector-assignable variables
    [SerializeField] private ushort coolDownTime;           // Time between weapon firing
    [SerializeField] private byte projectileRange;          // How far the projectile can go
    [SerializeField] private byte fullMagCount;             // Number of rounds in a full magazine for the type that's currently being used

    private AudioSource shotAudio;                          // The sound of the weapon firing
    private bool cooling;                                   // Creates a delay flag between shots
    private const byte bulletTrailStartPosition = 4;        // Where the bullet trail will start
    private GameObject bulletTrail;                         // The line left behind by the bullet
    private GameObject hitGameObject;                       // The actual gameobject that was hit
    private byte currentMagCount;                           // Current rounds in magazine
    private long timeStartedCooling;                        // When the weapon started cooling
    private Ray projectileFlightPath;
    private RaycastHit hitObjectRaycastInformation;         // Information about the raycast's hit
    private Transform muzzleTransform;                      // The transform of the muzzle on this weapon
    private ushort[] screenHalfHeightWidth;                 // 0 - Height. 1 - Width
    private Vector3 target;                                 // Where the bullet trails and raycasts will end
    #endregion

    #region Initialization
    private void Awake()
    {
        shotAudio = transform.GetChild(0).GetComponent<AudioSource>();
        cooling = false;
        currentMagCount = fullMagCount;                                                       
        muzzleTransform = transform.GetChild(0);                         
        screenHalfHeightWidth = new ushort[2] { (ushort)(Screen.height * 0.5f), (ushort)(Screen.width * 0.5f) };
    }
    #endregion

    #region Main Update
    public override void GameUpdate()
    {
        // If weapon is cooling
        if (cooling)
        {
            // If weapon has exceeding its cooling time, it is no longer cooling
            if (Cacher.gameTime.ElapsedMilliseconds - timeStartedCooling > coolDownTime)
                cooling = false;
        }
    }
    #endregion

    #region Public Interface
    public void Fire(byte _index)
    {
        // If weapon is not cooling
        if (!cooling)
        {
            // If there are rounds in the magazine
            if (currentMagCount > 0)
            {
                // Setting the position that the raycast will start from
                projectileFlightPath = Camera.main.ScreenPointToRay(new Vector2(screenHalfHeightWidth[1], screenHalfHeightWidth[0]));

                //Starts from the camera and flies straight
                //If raycast hits a target
                if (Physics.Raycast(projectileFlightPath.origin, projectileFlightPath.direction, out hitObjectRaycastInformation, projectileRange))
                {
                    target = hitObjectRaycastInformation.point;

                    // Stroing the hit game object
                    hitGameObject = hitObjectRaycastInformation.collider.gameObject;

                    if (hitGameObject.tag == "Enemy")
                        hitGameObject.GetComponent<EnemyBase>().TakeDamage();
                }

                //If raycast misses a target
                else
                    target = projectileFlightPath.origin + (projectileFlightPath.direction * projectileRange);

                //Find a bullet trail that's not beig used
                bulletTrail = Spawn.ReturnSpawnableAsset(Cacher.pooledAssets[0]);

                // Set positions, start line's life time, and turn it on
                bulletTrail.GetComponent<BulletTrailBehavior>().SetAttribute(muzzleTransform.position + (projectileFlightPath.direction * bulletTrailStartPosition), target);
                bulletTrail.SetActive(true);

                // Plays the audio for the shot
                shotAudio.Play();

                // Decrement after shot
                --currentMagCount;

                // Let the HUD know of this decrement
                Cacher.hudManager.UpdateRoundCount(_index, currentMagCount);

                // Weapon is now cooling after shot
                cooling = true;

                // Stores the start of the cooling operationtime
                timeStartedCooling = Cacher.gameTime.ElapsedMilliseconds;
            }
        }
    }
    public byte GetCurrentMagCount() { return currentMagCount; }
    public void ReplenishRounds(byte _index)
    {
        // Replenish back to whatever fullmag is
        currentMagCount = fullMagCount;

        // Let the HUD know of this reload
        Cacher.hudManager.UpdateRoundCount(_index, currentMagCount);
    }
    #endregion
}