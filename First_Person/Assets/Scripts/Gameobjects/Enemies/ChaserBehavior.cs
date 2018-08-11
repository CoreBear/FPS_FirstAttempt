using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserBehavior : EnemyBase
{
    #region Main Update
    public override void GameUpdate() { ChasePlayer(); }
    #endregion

    #region Blackbox
    private void ChasePlayer()
    {
        // Look at player
        transform.rotation = Quaternion.LookRotation(Cacher.characterTransform.position - transform.position);

        // Move toward player
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
    #endregion
}
