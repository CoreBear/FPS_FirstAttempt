using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Variables
    private ControllerInput controllerInput;

    // State Machine
    private GameStates[] availableStates;
    private GameStates currentState;

    private Stopwatch gameTime;
    #endregion

    #region Initialization
    private void Awake()
    {
        controllerInput = new ControllerInput();
        controllerInput.Awake(this);
        availableStates = new GameStates[2] { new Gameplay(this), new Paused() };
        currentState = availableStates[0];
        gameTime = new Stopwatch();
        gameTime.Start();
    }
    private void Start()
    {
        // Gives time for objects already in scene to be initialized
        controllerInput.Start();
    }
    #endregion

    #region Main Game Updates
    // Main update
    private void Update()
    {
        // Controller will be updated regardless of game state
        controllerInput.GameUpdate();

        // Runs the state machine for the entire game
        currentState.Update();
    }

    // Updates the majority of the game
    public void GameUpdate()
    {
        // Update each gameobject
        foreach (GameObjectBase objectScript in Cacher.pauseableGameObjects)
            objectScript.GameUpdate();
    }
    #endregion

    #region Public Interface
    public Stopwatch GetGameTime() { return gameTime; }
    public void TogglePause()
    {
        // Storage for toggle decision
        byte _index;

        // If game wasn't paused
        if (currentState == availableStates[0])
        {
            _index = 1;
            gameTime.Stop();
            Cacher.character.GetComponent<CharacterManager>().PauseCharacter();
        }

        // If game was paused
        else
        {
            _index = 0;
            gameTime.Start();
            Cacher.character.GetComponent<CharacterManager>().UnpauseCharacter();
        }

        // Change game state and controller input state
        currentState = availableStates[_index];
        controllerInput.ChangeInputStates(_index);
    }
    #endregion
}