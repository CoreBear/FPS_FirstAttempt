using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : GameObjectBase
{
    #region Variables
    // Inspector-assignable variables
    [SerializeField] protected byte moveSpeed;
    [SerializeField] private ushort scoreGiven;         // Score given when killed

    private bool disablingAtEndOfFrame;                 // A flag that will stop multiple calls to disabling coroutine function
    #endregion

    #region Initialization
    private void OnEnable() { disablingAtEndOfFrame = false; }
    #endregion

    #region Main Update
    public override void GameUpdate() { return; }
    #endregion

    #region Public Interface
    public void TakeDamage()
    {
        if (!disablingAtEndOfFrame)
        {
            disablingAtEndOfFrame = true;

            // Increase score with death
            Cacher.AlterScore(scoreGiven);

            StartCoroutine(Disable());
        }
    }
    #endregion

    #region Blackbox
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Explosion")
            StartCoroutine(Disable());
    }
    #endregion
}
