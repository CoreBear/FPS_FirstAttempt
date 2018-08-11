using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStates
{
    #region Main Update
    public virtual void Update() { return; }
    #endregion
}
public class Gameplay : GameStates
{
    #region Variables
    private delegate void GameController_GameUpdate();           // Delegate for main game update
    private GameController_GameUpdate GameUpdate;
    #endregion

    #region Initialization
    public Gameplay(GameController _gameController) { GameUpdate = _gameController.GameUpdate; }
    #endregion

    #region Main Update
    public override void Update() { GameUpdate(); }
    #endregion
}
public class Initialization : GameStates
{
    // Future implementation
}
public class Paused : GameStates
{
    // Future implementation
}
public class Death : GameStates
{
    // Future implementation
}
public class GameOver : GameStates
{
    // Future implementation
}