using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : GameObjectBase
{
    #region Variables
    // Inspector-assignable variables
    [SerializeField] private float growthSpeed;

    private AudioSource explosionAudio;

    // Change back to a bool if you go back to radius size
    private ushort terminationSize;                             // The size the explosion will disapate at
    //private float initialRadiusSize;
    #endregion

    #region Initialization
    private void Awake()
    {
        explosionAudio = GetComponent<AudioSource>();
        //initialRadiusSize = GetComponent<SphereCollider>().radius;
    }
    private void OnEnable()
    {
        // Use explosion in set attributes
        explosionAudio.Play();
    }
    #endregion

    #region Main Update
    public override void GameUpdate()
    {
        // Grow explosion speed at a specified rate
        //GetComponent<SphereCollider>().radius *= growthSpeed;
        transform.localScale *= growthSpeed;

        // When it's grown a certain size, disable it
        //if (GetComponent<SphereCollider>().radius > terminationSize)
        //    StartCoroutine(Disable());

        if (transform.localScale.x > terminationSize)
            StartCoroutine(Disable());
    }
    #endregion

    #region Public Interface
    public void SetAttributes(ushort _terminationSize, Vector3 _startPosition)
    {
        //GetComponent<SphereCollider>().radius = initialRadiusSize;
        transform.localScale = new Vector3(6, 6, 6);
        terminationSize = _terminationSize;
        transform.position = _startPosition;
        Cacher.pauseableGameObjects.Add(this);
        //explosionAudio.Play(); 
    }
    #endregion
}