using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CONTROLLER_BUTTON{BOTTOM_FACE, DPAD_LEFT, DPAD_UP, DPAD_RIGHT, DPAD_DOWN, JOY_STICK_LEFT, JOY_STICK_RIGHT, L1B, L2T, L3, LEFT_FACE, OPTIONS, PS_BUTTON, R1B, R2T, R3, RIGHT_FACE, SHARE, TOP_FACE, TOUCH_PAD };

public class ControllerInput
{
    #region Variables
    private CharacterManager characterManager;

    // Storing joystick positions
    private float[] leftStickPositions;
    private float[] rightStickPositions;

    // State Machine
    private InputStates[] availableStates;
    private InputStates currentState;

    private delegate void GameController_TogglePause();
    private GameController_TogglePause TogglePause;
    //private sbyte[] dPadButtonPressed;                      // Storing d-pad presses
    #endregion

    #region Initialization
    public void Awake(GameController _gameController)
    {
        leftStickPositions = new float[2] { 0, 0 };
        rightStickPositions = new float[2] { 0, 0 };
        availableStates = new InputStates[2] { new GameInput(this), new PausedInput(this) };
        currentState = availableStates[0];
        TogglePause = _gameController.TogglePause;
        //dPadButtonPressed = new sbyte[2] { 0, 0 };
    }
    public void Start()
    {
        // Downcasting from gameobject base
        characterManager = (CharacterManager)Cacher.pauseableGameObjects[0];

        // Sending this instance to the character manager so the joystick delegates can be set up
        characterManager.AssignStickFunctionality(this);
    }
    #endregion

    #region Public Interface
    public void ChangeInputStates(byte _index) { currentState = availableStates[_index]; }

    // These will be called by the character manager via delegates
    public float[] GetLeftStickPositions() { return leftStickPositions; }
    public float[] GetRightStickPositions() { return rightStickPositions; }
    #endregion

    #region Main Update
    public void GameUpdate() { currentState.GameUpdate(); }
    public void RunControllerGame()
    {
        // Which controller to run
        //RunControllerGame360();
        RunControllerGamePS4();
    }
    public void RunControllerPaused()
    {
        // Which controller to run
        //RunControllerPaused360();
        RunControllerPausedPS4();
    }
    #endregion

    #region Controller
    #region Controller Update
    #region PS4
    public void RunControllerGamePS4()
    {
        // Have to call d-pad for it to work
        // Runs the left stick
        if (!LeftJoystickUnpausedPS4())
        {
            // If left stick is centered, change character state to idle
            if (characterManager.CheckState(1))
                characterManager.ChangeCharacterStates(0);
        }

        // Runs right stick
        RightJoystickPS4();
        
        // If any button on the controller is pressed
        if (Input.anyKey)
        {
            // GetKey - Down during frame
            // GetKeyDown - Down on frame
            // GetKeyUp - Up on frame

            if (Input.GetKeyDown("joystick button 0"))
                LeftFace();
            if (Input.GetKeyDown("joystick button 2"))
                RightFaceUnpaused();
            if (Input.GetKeyDown("joystick button 3"))
                TopFace();
            if (Input.GetKeyDown("joystick button 4"))
                L1B();
            if (Input.GetKeyDown("joystick button 5"))
                R1B();
            if (Input.GetKey("joystick button 6"))
                L2T();
            if (Input.GetKey("joystick button 7"))
                R2T();
            if (Input.GetKeyDown("joystick button 8"))
                BackShare();
            if (Input.GetKeyDown("joystick button 9"))
                OptionStart();
            if (Input.GetKeyDown("joystick button 10"))
                L3();
            if (Input.GetKeyDown("joystick button 11"))
                R3();
            if (Input.GetKeyDown("joystick button 12"))
                PSButton();
            if (Input.GetKeyDown("joystick button 13"))
                TouchPad();
        }
    }
    public void RunControllerPausedPS4()
    {
        // Runs left stick
        LeftJoystickPaused();
        
        if (Input.anyKey)
        {
            if (Input.GetKeyDown("joystick button 1"))
                BottomFacePaused();
            if (Input.GetKeyDown("joystick button 2"))
                RightFacePaused();
            if (Input.GetKeyDown("joystick button 9"))
                OptionStart();
        }
    }
    #endregion

    #region 360
    public void RunControllerGame360()
    {
        // Have to call d-pad for it to work
        // Runs the left stick
        if (!LeftJoystickUnpaused360())
        {
            // If left stick is centered, change character state to idle
            if (characterManager.CheckState(1))
                characterManager.ChangeCharacterStates(0);
        }

        // Runs right stick
        RightJoystick360();

        RunTriggers360();

        // If any button on the controller is pressed
        if (Input.anyKey)
        {
            // GetKey - Down during frame
            // GetKeyDown - Down on frame
            // GetKeyUp - Up on frame

            if (Input.GetKeyDown("joystick button 1"))
                RightFaceUnpaused();
            if (Input.GetKeyDown("joystick button 2"))
                LeftFace();
            if (Input.GetKeyDown("joystick button 3"))
                TopFace();
            if (Input.GetKeyDown("joystick button 4"))
                L1B();
            if (Input.GetKeyDown("joystick button 5"))
                R1B();
            if (Input.GetKeyDown("joystick button 6"))
                BackShare();
            if (Input.GetKeyDown("joystick button 7"))
                OptionStart();
            if (Input.GetKeyDown("joystick button 8"))
                L3();
            if (Input.GetKeyDown("joystick button 9"))
                R3();
        }
    }
    public void RunControllerPaused360()
    {
        // Runs left stick
        LeftJoystickPaused();

        if (Input.anyKey)
        {
            if (Input.GetKeyDown("joystick button 0"))
                BottomFacePaused();
            if (Input.GetKeyDown("joystick button 1"))
                RightFacePaused();
            if (Input.GetKeyDown("joystick button 7"))
                OptionStart();
        }
    }
    #endregion
    #endregion

    #region Bumpers & Triggers
    private void L1B() { characterManager.ControllerRequest(CONTROLLER_BUTTON.L1B); }
    private void L2T() { characterManager.ControllerRequest(CONTROLLER_BUTTON.L2T); }
    private void R1B() { characterManager.ControllerRequest(CONTROLLER_BUTTON.R1B); }
    private void R2T() { characterManager.ControllerRequest(CONTROLLER_BUTTON.R2T); }
    private void RunTriggers360()
    {
        if (Input.GetAxisRaw("LeftTrigger360") != 0)
            L2T();
        if (Input.GetAxisRaw("RightTrigger360") != 0)
            R2T();
    }
    #endregion

    #region D-Pad & Joysticks
    private void DPad360()
    {
        //dPadButtonPressed[0] = (sbyte)Input.GetAxisRaw("DPadHorizontal360");
        //dPadButtonPressed[1] = (sbyte)Input.GetAxisRaw("DPadVertical360");

        //RunDpad();
    }
    private void DPadPS4()
    {
        //dPadButtonPressed[0] = (sbyte)Input.GetAxisRaw("DPadHorizontalPS4");
        //dPadButtonPressed[1] = (sbyte)Input.GetAxisRaw("DPadVerticalPS4");

        //RunDpad();
    }
    private void RunDpad()
    {
        //// If D-pad is being pressed
        //if (dPadButtonPressed[0] != 0 || dPadButtonPressed[1] != 0)
        //{
        //    // Horizontal d-pad buttons
        //    if (dPadButtonPressed[0] < 0)
        //        characterManager.ControllerRequest(CONTROLLER_BUTTON.DPAD_LEFT);
        //    else
        //        characterManager.ControllerRequest(CONTROLLER_BUTTON.DPAD_RIGHT);

        //    // Vertical d-pad buttons
        //    if (dPadButtonPressed[1] < 0)
        //        characterManager.ControllerRequest(CONTROLLER_BUTTON.DPAD_DOWN);
        //    else
        //        characterManager.ControllerRequest(CONTROLLER_BUTTON.DPAD_UP);

        //    return true;
        //}

        // If d-pad is not being pressed
        //return false;
    }
    private void L3() { characterManager.ControllerRequest(CONTROLLER_BUTTON.L3); }
    private void LeftJoystickPaused()
    {
        // Store stick position during each frame
        leftStickPositions[0] = Input.GetAxis("LeftJoystickHorizontal");
        leftStickPositions[1] = Input.GetAxis("LeftJoystickVertical");

        // If stick isn't centered
        if (leftStickPositions[0] != 0 || leftStickPositions[1] != 0)
        {
            Debug.Log("LeftStickPaused");
            //return true;
        }

        // If right stick is centered
        //return false;
    }
    private bool LeftJoystickUnpaused360()
    {
        // Store stick position during each frame
        leftStickPositions[0] = Input.GetAxis("LeftJoystickHorizontal");
        leftStickPositions[1] = Input.GetAxis("LeftJoystickVertical");

        // If stick isn't centered
        if (leftStickPositions[0] != 0 || leftStickPositions[1] != 0)
        {
            characterManager.ControllerRequest(CONTROLLER_BUTTON.JOY_STICK_LEFT);
            
            // If X is pressed while moving in a direction
            if (Input.GetKeyDown("joystick button 0"))
                BottomFaceUnpaused();

            return true;
        }

        // If right stick is centered
        return false;
    }
    private bool LeftJoystickUnpausedPS4()
    {
        // Store stick position during each frame
        leftStickPositions[0] = Input.GetAxis("LeftJoystickHorizontal");
        leftStickPositions[1] = Input.GetAxis("LeftJoystickVertical");

        // If stick isn't centered
        if (leftStickPositions[0] != 0 || leftStickPositions[1] != 0)
        {
            characterManager.ControllerRequest(CONTROLLER_BUTTON.JOY_STICK_LEFT);
            
            // If X is pressed while moving in a direction
            if (Input.GetKeyDown("joystick button 1"))
                BottomFaceUnpaused();

            return true;
        }

        // If right stick is centered
        return false;
    }
    private void R3() { characterManager.ControllerRequest(CONTROLLER_BUTTON.R3); }
    private void RightJoystick360()
    {
        // Store stick position during each frame
        rightStickPositions[0] = Input.GetAxis("RightJoystickHorizontal360");
        rightStickPositions[1] = Input.GetAxis("RightJoystickVertical360");

        // If stick isn't centered
        if (rightStickPositions[0] != 0 || rightStickPositions[1] != 0)
        {
            characterManager.ControllerRequest(CONTROLLER_BUTTON.JOY_STICK_RIGHT);
            //return true;
        }

        // If right stick is centered
        //return false;
    }
    private void RightJoystickPS4()
    {
        // Store stick position during each frame
        rightStickPositions[0] = Input.GetAxis("RightJoystickHorizontalPS4");
        rightStickPositions[1] = Input.GetAxis("RightJoystickVerticalPS4");

        // If stick isn't centered
        if (rightStickPositions[0] != 0 || rightStickPositions[1] != 0)
        {
            characterManager.ControllerRequest(CONTROLLER_BUTTON.JOY_STICK_RIGHT);
            //return true;
        }

        // If right stick is centered
        //return false;
    }
    #endregion

    #region Face Buttons
    // Circle - PS4. B - 360
    private void RightFacePaused() { Debug.Log("RightFacePaused"); }
    private void RightFaceUnpaused() { characterManager.ControllerRequest(CONTROLLER_BUTTON.RIGHT_FACE); }
    
    // Square - PS4. X - 360
    private void LeftFace() { characterManager.ControllerRequest(CONTROLLER_BUTTON.LEFT_FACE); }

    // Triangle - PS4. Y - 360
    private void TopFace() { characterManager.ControllerRequest(CONTROLLER_BUTTON.TOP_FACE); }

    // X - PS4. Y - 360
    private void BottomFacePaused() { Debug.Log("BottomFacePaused"); }
    private void BottomFaceUnpaused() { characterManager.ControllerRequest(CONTROLLER_BUTTON.BOTTOM_FACE); }
    #endregion

    #region Misc Buttons
    // Option - PS4. Start - 360
    private void OptionStart() { TogglePause(); }
    private void PSButton() { Debug.Log("PS Button Pressed"); }

    // Share - PS4. Back - 360
    private void BackShare() { Debug.Log("BackShare Button Pressed"); }
    private void TouchPad() { Debug.Log("Touchpad Pressed"); }
    #endregion
    #endregion
}