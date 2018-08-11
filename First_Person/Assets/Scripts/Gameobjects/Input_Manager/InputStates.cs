using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputStates
{
    #region Main Update
    public abstract void GameUpdate();
    #endregion
}
public class GameInput : InputStates
{
    #region Variables
    private delegate void ControllerInput_RunControllerGame();
    private ControllerInput_RunControllerGame RunControllerGame;
    #endregion

    #region Initialization
    public GameInput(ControllerInput _controllerInput) { RunControllerGame = _controllerInput.RunControllerGame; }
    #endregion

    #region Main Update
    public override void GameUpdate() { RunControllerGame(); }
    #endregion
}
public class PausedInput : InputStates
{
    #region Variables
    private delegate void ControllerInput_RunControllerPaused();
    private ControllerInput_RunControllerPaused RunControllerPaused;
    #endregion

    #region Initialization
    public PausedInput(ControllerInput _controllerInput) { RunControllerPaused = _controllerInput.RunControllerPaused; }
    #endregion

    #region Main Update
    public override void GameUpdate() { RunControllerPaused(); }
    #endregion
}