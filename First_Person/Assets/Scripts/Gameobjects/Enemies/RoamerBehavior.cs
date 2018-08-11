using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamerBehavior : EnemyBase
{
    #region Variables
    private Vector3 moveDirection;
    #endregion

    #region Initialization
    private void OnEnable()
    {
        // Randomly choose a direction
        switch (Random.Range(0, 4))
        {
            case 0:
                moveDirection = Vector3.left;
                break;
            case 1:
                moveDirection = Vector3.forward;
                break;
            case 2:
                moveDirection = Vector3.right;
                break;
            case 3:
                moveDirection = Vector3.back;
                break;
        }
    }
    #endregion

    #region Main Update
    public override void GameUpdate() { Roam(); }
    #endregion

    #region Blackbox
    private void ChangeDirection()
    {
        if (moveDirection == Vector3.left)
            moveDirection = Vector3.right;
        else if (moveDirection == Vector3.forward)
            moveDirection = Vector3.back;
        else if (moveDirection == Vector3.right)
            moveDirection = Vector3.left;
        else if (moveDirection == Vector3.back)
            moveDirection = Vector3.forward;
    }
    protected override void OnTriggerEnter(Collider _other)
    {
        if (_other.tag == "Wall")
            ChangeDirection();

        base.OnTriggerEnter(_other);
    }
    private void Roam()
    {
        // Move toward in random direction
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
    #endregion
}
