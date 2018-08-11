using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStates
{
    #region Variables
    protected CharacterManager_SetMoveSpeed SetMoveSpeed;
    protected delegate void CharacterManager_SetMoveSpeed(bool _walking);
    #endregion

    #region Initialization
    public CharacterStates(CharacterManager _characterManager) { SetMoveSpeed = _characterManager.SetMoveSpeed; }
    public virtual void Enable() { return; }
    #endregion

    #region Main Update
    public virtual void Update() { return; }
    #endregion

    #region PUblic Functionality
    public virtual void Disable() { return; }
    public virtual void Move() { return; }
    #endregion
}
public class Idle : CharacterStates
{
    #region Variables
    private CharacterManager_ChangeCharacterStates ChangeCharacterStates;
    private CharacterManager_StateUpdate StateUpdate;
    private CharacterManager_StopMovementAudio StopMovementAudio;
    private delegate void CharacterManager_ChangeCharacterStates(byte _index);
    private delegate void CharacterManager_StateUpdate();
    private delegate void CharacterManager_StopMovementAudio();
    #endregion

    #region Initialization
    public Idle(CharacterManager _characterManager) : base(_characterManager)
    {
        ChangeCharacterStates = _characterManager.ChangeCharacterStates;
        StateUpdate = _characterManager.StateUpdate;
        StopMovementAudio = _characterManager.StopMovementAudio;
    }
    public override void Enable() { StopMovementAudio(); }
    #endregion

    #region Main Update
    public override void Update() { StateUpdate(); }
    #endregion

    #region Public Interface
    public override void Move() { ChangeCharacterStates(1); }
    #endregion
}
public class Walking : CharacterStates
{
    #region Variables
    private CharacterManager_MoveCharacter MoveCharacter;
    private CharacterManager_StartMovementAudio StartMovementAudio;
    private CharacterManager_StartTimeStartedCooling StartTimeStartedCooling;
    private CharacterManager_StateUpdate StateUpdate;
    private delegate void CharacterManager_MoveCharacter();
    private delegate void CharacterManager_StartMovementAudio();
    private delegate void CharacterManager_StartTimeStartedCooling();
    private delegate void CharacterManager_StateUpdate();
    #endregion

    #region Initialization
    public Walking(CharacterManager _characterManager) : base(_characterManager)
    {
        MoveCharacter = _characterManager.MoveCharacter;
        StartMovementAudio = _characterManager.StartMovementAudio;
        StartTimeStartedCooling = _characterManager.StartTimeStartedCooling;
        StateUpdate = _characterManager.StateUpdate;
    }
    public override void Enable()
    {
        SetMoveSpeed(true);
        StartTimeStartedCooling();
        StartMovementAudio();
    }
    #endregion

    #region Public Functionality
    public override void Move() { MoveCharacter(); }
    public override void Update() { StateUpdate(); }
    #endregion
}
public class Dashing : CharacterStates
{
    #region Variables
    private CharacterManager_Dash Dash;
    private CharacterManager_StartCooling StartCooling;
    private CharacterManager_StartMovementAudio StartMovementAudio;
    private delegate void CharacterManager_Dash();
    private delegate void CharacterManager_StartCooling();
    private delegate void CharacterManager_StartMovementAudio();
    #endregion

    #region Initialization
    public Dashing(CharacterManager _characterManager) : base(_characterManager)
    {
        Dash = _characterManager.Dash;
        StartCooling = _characterManager.StartCooling;
        StartMovementAudio = _characterManager.StartMovementAudio;
    }
    public override void Enable()
    {
        SetMoveSpeed(false);
        StartMovementAudio();
    }
    #endregion

    #region Main Update
    public override void Update() { Dash(); }
    #endregion

    #region Public Functionality
    public override void Disable() { StartCooling(); }
    #endregion
}
public class CharacterPaused : CharacterStates
{
    #region Variables
    private CharacterManager_PauseMovementAudio PauseMovementAudio;
    private CharacterManager_UnpauseMovementAudio UnpauseMovementAudio;
    private delegate void CharacterManager_PauseMovementAudio();
    private delegate void CharacterManager_UnpauseMovementAudio();
    #endregion

    #region Initialization
    public CharacterPaused(CharacterManager _characterManager) : base(_characterManager)
    {
        PauseMovementAudio = _characterManager.PauseMovementAudio;
        UnpauseMovementAudio = _characterManager.UnpauseMovementAudio;
    }
    public override void Enable() { PauseMovementAudio(); }
    #endregion

    #region Public Functionality
    public override void Disable() { UnpauseMovementAudio(); }
    #endregion
}
