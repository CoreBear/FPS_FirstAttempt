using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CONTROLLER_BUTTON{CIRCLE, DPAD_LEFT, DPAD_UP, DPAD_RIGHT, DPAD_DOWN, JOY_STICK_LEFT, JOY_STICK_RIGHT, L1, L2, L3, OPTIONS, PS_BUTTON, R1, R2, R3, SHARE, SQUARE, TOUCH_PAD, TRIANGLE, X };
public class ControllerInput : MonoBehaviour
{
    // Button layout ideas
    // circle is left reload and triangle is right reload
    // left bumper is left something and right bumper is right something. Maybe gun change
    // What could circle be used for?
    // Bomb is r3

    #region Variables
    // Do this better. May not be needed
    private bool[] inputChecks;                             // 0 - D-pad. 1 - Left Joy Stick. 2 - Right Joy Stick. 3 - Buttons

    private byte iterator;

    // Storing joystick positions
    private CharacterManager characterManager;
    private float[] leftStickPositions;
    private float[] rightStickPositions;
    private sbyte[] dPadButtonPressed;
    #endregion

    #region Initialization
    private void Awake()
    {
        inputChecks = new bool[4] { false, false, false, false };
        leftStickPositions = new float[2] { 0, 0 };
        rightStickPositions = new float[2] { 0, 0 };
        dPadButtonPressed = new sbyte[2] { 0, 0 };
    }
    private void Start()
    {
        // Dirty assigning of character manager
        // Do this better and for each player
        characterManager = GameObject.Find("Player").GetComponent<CharacterManager>();

        // Sending this instance to the character manager so the joystick delegates can be set up
        characterManager.AssignStickFunctionality(this);
    }
    #endregion

    #region Public Interface
    // These will be called by the character manager via delegates
    public float[] GetLeftStickPositions() { return leftStickPositions; }
    public float[] GetRightStickPositions() { return rightStickPositions; }
    #endregion

    #region Main Update
    private void Update()
    {
        RunPS4Controller();
    }
    #endregion

    #region PS4
    #region Controller Update
    private void RunPS4Controller()
    {
        inputChecks[0] = DPad();
        inputChecks[1] = LeftJoystick();
        inputChecks[2] = RightJoystick();
        inputChecks[3] = Input.anyKey;

        // If character is grounded
        if (characterManager.GetCurrentState() == characterManager.GetRequestedState(0))
        {
            // If no bumper, button, stick, or trigger is being pressed
            if (!inputChecks[0] && !inputChecks[1] && !inputChecks[2] && !inputChecks[3])
            {
                // Change state to idle
                characterManager.ChangeCharacterStates(0, 0);

                // Resetting checks
                for (iterator = 0; iterator < inputChecks.Length; ++iterator)
                    inputChecks[iterator] = false;

                return;
            }
        }

        // If character is not grounded
        else
        {
            // If joysticks are centered
            if (!inputChecks[1] && !inputChecks[2])
            {
                // Change horizontal speed to walking and change previous state to walking for the landgin
                characterManager.ChangeAxisSpeed(0, MOVEMENT_SPEED.WALK);
                characterManager.StorePreviousGroundingState(1);

                // Resetting checks
                for (iterator = 0; iterator < inputChecks.Length; ++iterator)
                    inputChecks[iterator] = false;

                // If no bumper, button, stick, or trigger is being pressed
                if (!inputChecks[0] && !inputChecks[3])
                    return;
            }
        }

        // If any button on the controller is pressed
        if (Input.anyKey)
        {
            // GetKey - Down during frame
            // GetKeyDown - Down on frame
            // GetKeyUp - Up on frame

            if (Input.GetKeyDown("joystick button 0"))
                Square();
            if (Input.GetKeyDown("joystick button 1"))
                X();
            if (Input.GetKeyDown("joystick button 2"))
                Circle();
            if (Input.GetKeyDown("joystick button 3"))
                Triangle();
            if (Input.GetKeyDown("joystick button 4"))
                L1();
            if (Input.GetKeyDown("joystick button 5"))
                R1();
            if (Input.GetKeyDown("joystick button 6"))
                L2();
            if (Input.GetKeyDown("joystick button 7"))
                R2();
            if (Input.GetKeyDown("joystick button 8"))
                Share();
            if (Input.GetKeyDown("joystick button 9"))
                Option();
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
    #endregion

    #region Bumpers & Triggers
    private void L1() { characterManager.ControllerRequest(CONTROLLER_BUTTON.L1); }
    private void L2() { characterManager.ControllerRequest(CONTROLLER_BUTTON.L2); }
    private void R1() { characterManager.ControllerRequest(CONTROLLER_BUTTON.R1); }
    private void R2() { characterManager.ControllerRequest(CONTROLLER_BUTTON.R2); }
    #endregion

    #region D-Pad & Joysticks
    private bool DPad()
    {
        dPadButtonPressed[0] = (sbyte)Input.GetAxisRaw("DPadHorizontal");
        dPadButtonPressed[1] = (sbyte)Input.GetAxisRaw("DPadVertical");

        // If D-pad is being pressed
        if (dPadButtonPressed[0] != 0 || dPadButtonPressed[1] != 0)
        {
            // Horizontal d-pad buttons
            if (dPadButtonPressed[0] < 0)
                characterManager.ControllerRequest(CONTROLLER_BUTTON.DPAD_LEFT);
            else
                characterManager.ControllerRequest(CONTROLLER_BUTTON.DPAD_RIGHT);

            // Vertical d-pad buttons
            if (dPadButtonPressed[1] < 0)
                characterManager.ControllerRequest(CONTROLLER_BUTTON.DPAD_DOWN);
            else
                characterManager.ControllerRequest(CONTROLLER_BUTTON.DPAD_UP);

            return true;
        }

        // If d-pad is not being pressed
        return false;
    }
    private void L3() { characterManager.ControllerRequest(CONTROLLER_BUTTON.L3); }
    private bool LeftJoystick()
    {
        // Store stick position during each frame
        leftStickPositions[0] = Input.GetAxis("LeftJoystickHorizontal");
        leftStickPositions[1] = Input.GetAxis("LeftJoystickVertical");

        // If stick isn't centered
        if (leftStickPositions[0] != 0 || leftStickPositions[1] != 0)
        {
            characterManager.ControllerRequest(CONTROLLER_BUTTON.JOY_STICK_LEFT);
            return true;
        }

        // If right stick is centered
        return false;
    }
    private void R3() { characterManager.ControllerRequest(CONTROLLER_BUTTON.R3); }
    private bool RightJoystick()
    {
        // Store stick position during each frame
        rightStickPositions[0] = Input.GetAxis("RightJoystickHorizontal");
        rightStickPositions[1] = Input.GetAxis("RightJoystickVertical");

        // If stick isn't centered
        if (rightStickPositions[0] != 0 || rightStickPositions[1] != 0)
        {
            characterManager.ControllerRequest(CONTROLLER_BUTTON.JOY_STICK_RIGHT);
            return true;
        }

        // If right stick is centered
        return false;
    }
    #endregion

    #region Face Buttons
    private void Circle() { characterManager.ControllerRequest(CONTROLLER_BUTTON.CIRCLE); }
    private void Square() { characterManager.ControllerRequest(CONTROLLER_BUTTON.SQUARE); }
    private void Triangle() { characterManager.ControllerRequest(CONTROLLER_BUTTON.TRIANGLE); }
    private void X() { characterManager.ControllerRequest(CONTROLLER_BUTTON.X); }
    #endregion

    #region Misc Click
    private void Option() { Debug.Log("Option Button Pressed"); }
    private void PSButton() { Debug.Log("PS Button Pressed"); }
    private void Share() { Debug.Log("Share Button Pressed"); }
    private void TouchPad() { Debug.Log("Touchpad Pressed"); }
    #endregion
    #endregion
}