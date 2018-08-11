using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrailBehavior : GameObjectBase
{
    #region Variables
    // Inspector-assignable variables
    [SerializeField] private byte timeAlive;

    private long timeEnabled;
    #endregion

    #region Initialization
    private void Awake()
    {
        // Initializing attributes
        GetComponent<LineRenderer>().material.color = Color.yellow;
        GetComponent<LineRenderer>().startWidth = .1f;
        GetComponent<LineRenderer>().endWidth = .1f;
    }
    #endregion

    #region Main Update
    public override void GameUpdate()
    {
        if (Cacher.gameTime.ElapsedMilliseconds - timeEnabled > timeAlive)
            StartCoroutine(Disable());
    }
    #endregion

    #region Public Interface
    public void SetAttribute(Vector3 _startingPosition, Vector3 _endingPosition)
    {
        GetComponent<LineRenderer>().SetPosition(0, _startingPosition);
        GetComponent<LineRenderer>().SetPosition(1, _endingPosition);
        timeEnabled = Cacher.gameTime.ElapsedMilliseconds;
        Cacher.pauseableGameObjects.Add(this);
    }
    #endregion
}
