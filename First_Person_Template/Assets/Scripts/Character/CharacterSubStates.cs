using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterSubStates
{
    #region Variables
    protected CharacterManager characterManager;
    protected CharacterStates characterStates;                                                  // Conncetion to the state above
    #endregion

    #region Initialization
    public CharacterSubStates(CharacterStates _characterStates, CharacterManager _characterManager)
    {
        // Function pointer that will change character's speed between state changes
        characterManager = _characterManager;
        characterStates = _characterStates;
    }
    #endregion

    #region Main Update
    public virtual void Update() { return; }
    #endregion

    #region Public Interface
    public virtual void Enable() { return; }
    public virtual void Jump() { return; }
    public virtual void Move() { characterManager.MoveCharacter(); }
    public virtual void ToggleRun() { return; }
    #endregion
}

// Grounded substates
public class Idle : CharacterSubStates
{
    #region Initialization
    public override void Enable()
    {
        // 0 for horizontal movement
        characterManager.ChangeAxisSpeed(0, MOVEMENT_SPEED.WALK);
    }
    public Idle(CharacterStates _characterStates, CharacterManager _characterManager) : base(_characterStates, _characterManager) { return; }
    #endregion

    #region Public Interface
    public override void Jump()
    {
        // Storing this state so character can return to it after jumping
        characterManager.StorePreviousGroundingState(1);

        // Changing character's state from grounded idle to airborne jumping
        characterManager.ChangeCharacterStates(1, 0);
    }
    public override void Move()
    {
        // Changing character's state from idle to walking
        characterStates.ChangeCharacterSubState(1);
    }
    #endregion
}
public class Walking : CharacterSubStates
{
    #region Initialization
    public override void Enable()
    {
        // 0 for horizontal movement
        characterManager.ChangeAxisSpeed(0, MOVEMENT_SPEED.WALK);
    }
    public Walking(CharacterStates _characterStates, CharacterManager _characterManager) : base(_characterStates, _characterManager) { return; }
    #endregion

    #region Public Interface
    public override void Jump()
    {
        // Storing this state so character can return to it after jumping
        characterManager.StorePreviousGroundingState(1);

        // Changing character's state from grounded walking to airborne jumping
        characterManager.ChangeCharacterStates(1, 0);
    }
    public override void ToggleRun()
    {
        // change substate to running
        characterStates.ChangeCharacterSubState(2);
    }
    #endregion
}
public class Running : CharacterSubStates
{
    #region Initialization
    public override void Enable()
    {
        // 0 for horizontal movement
        characterManager.ChangeAxisSpeed(0, MOVEMENT_SPEED.RUN);
    }
    public Running(CharacterStates _characterStates, CharacterManager _characterManager) : base(_characterStates, _characterManager) { return; }
    #endregion

    #region Public Interface
    public override void Jump()
    {
        // Storing this state so character can return to it after jumping
        characterManager.StorePreviousGroundingState(2);

        // Changing character's state from grounded running to airborne jumping
        characterManager.ChangeCharacterStates(1, 0);
    }
    public override void ToggleRun()
    {
        // Change substate to walking
        characterStates.ChangeCharacterSubState(1);
    }
    #endregion
}

// Airborne substates
public class Jumping : CharacterSubStates
{
    #region Initialization
    public override void Enable()
    {
        // 1 for vertical movement
        characterManager.ChangeAxisSpeed(1, MOVEMENT_SPEED.JUMP);
    }
    public Jumping(CharacterStates _characterStates, CharacterManager _characterManager) : base(_characterStates, _characterManager) { return; }
    #endregion

    #region Main Update
    public override void Update() { characterManager.Jump(); }
    #endregion
}
public class Falling : CharacterSubStates
{
    #region Initialization
    public Falling(CharacterStates _characterStates, CharacterManager _characterManager) : base(_characterStates, _characterManager) { return; }
    #endregion

    #region Main Update
    public override void Update() { characterManager.Fall(); }
    #endregion
}
