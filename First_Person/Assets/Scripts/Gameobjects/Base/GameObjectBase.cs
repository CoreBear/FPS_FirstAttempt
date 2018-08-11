using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameObjectBase : MonoBehaviour
{
    #region Main Update
    // Every game object must have this type of object, because if the game is paused, it won't be called
    // Object must be added to cacher's paused objects list for this function to be called
    public abstract void GameUpdate();
    #endregion

    #region Initialization
    public IEnumerator Enable()
    {
        // Waiting until the end of the frame to perform these functions, becuase gamecontroller's foreach loop will complain
        yield return new WaitForEndOfFrame();

        // Removing object from update list
        Cacher.pauseableGameObjects.Add(this);

        // Turns object off, instead of destoying memory
        gameObject.SetActive(true);
    }
    #endregion

    #region Public Interface
    public IEnumerator Disable()
    {
        // Waiting until the end of the frame to perform these functions, becuase gamecontroller's foreach loop will complain
        yield return new WaitForEndOfFrame();

        // Removing object from update list
        Cacher.pauseableGameObjects.Remove(this);

        // Turns object off, instead of destoying memory
        gameObject.SetActive(false);
    }
    public void InstaKill() { StartCoroutine(Disable()); }
    #endregion
}