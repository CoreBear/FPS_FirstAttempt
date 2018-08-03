using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStates
{
    #region Variables
    protected CharacterManager characterManager;        // Connection to the main character script
    protected CharacterSubStates[] availableStates;     // Different for each of the inheriting states
    protected CharacterSubStates currentState;
    #endregion

    #region Initialization
    public CharacterStates(CharacterManager _characterManager) { characterManager = _characterManager; }
    #endregion

    #region Main Update
    public void Update() { currentState.Update(); }
    #endregion

    #region Public Interface
    public void ChangeCharacterSubState(byte _index)
    {
        // Changes character's state to whatever is being passed in
        currentState = availableStates[_index];
        currentState.Enable();
    }
    public virtual void Jump() { return; }
    public void Move() { currentState.Move(); }
    public virtual void ToggleRun() { return; }
    #endregion
}
public class Grounded : CharacterStates
{
    #region Initialization
    public Grounded(CharacterManager _characterManager) : base (_characterManager)
    {
        availableStates = new CharacterSubStates[] { new Idle(this, _characterManager), new Walking(this, _characterManager), new Running(this, _characterManager) };
        currentState = availableStates[0];
    }
    #endregion

    #region Public Interface
    public override void Jump() { currentState.Jump(); }
    public override void ToggleRun() { currentState.ToggleRun(); }
    #endregion
}
public class Airborne : CharacterStates
{
    #region Initialization
    public Airborne(CharacterManager _characterManager) : base(_characterManager)
    {
        availableStates = new CharacterSubStates[] { new Jumping(this, _characterManager), new Falling(this, _characterManager) };
        currentState = availableStates[0];
    }
    #endregion
}