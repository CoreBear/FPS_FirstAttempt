using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MOVEMENT_SPEED{ JUMP, RUN, WALK };
public class CharacterManager : MonoBehaviour
{
    #region Variables
    // Inspecter assigned variables
    [SerializeField] private byte gravityStrength;
    [SerializeField] private byte jumpStrength;
    [SerializeField] private byte runSpeed;
    [SerializeField] private byte walkSpeed;
    [SerializeField] private byte horizontalRotationSpeed;
    [SerializeField] private byte verticalRotationSpeed;

    private byte characterStartingY;                       // Original height for jump calculations
    private byte previousGroundedState;                    // Character's state before jumping
    private delegate float[] JoysticInput();                
    private float[][] joystickInformation;                 // Storage location for joystick information
    private JoysticInput leftStickPosition;                // Function pointer to receive joystick direction information
    private JoysticInput rightStickPosition;               // Function pointer to receive joystick direction information
    private sbyte[] axisMovementSpeed;                     // 0 - Horizontal. 1 - Vertical

    // State Machine
    private CharacterStates[] availableStates;             // 0 - Grounded. 1 - Airborne
    private CharacterStates currentState;

    private Transform verticalRotatorTransform;            // Look up and down
    private Vector3 landingPosition;                       // Instead of creating a new vector each time to update character's landing position
    private Vector3 moveInformation;                       // The direction and speed of character movement
    #endregion

    #region Initialization
    private void Awake()
    {
        characterStartingY = (byte)transform.position.y;
        previousGroundedState = 0;
        axisMovementSpeed = new sbyte[] { (sbyte)walkSpeed, 0 };
        
        // Setting up joystick information storage
        joystickInformation = new float[2][];
        joystickInformation[0] = new float[2] { 0, 0 };
        joystickInformation[1] = new float[2] { 0, 0 };

        // Initializing states and setting character to grounded
        availableStates = new CharacterStates[] { new Grounded(this), new Airborne(this) };
        currentState = availableStates[0];

        verticalRotatorTransform = transform.GetChild(1).transform;
    }
    #endregion

    #region Main Update
    private void Update() { currentState.Update(); }
    #endregion

    #region Public Interface
    public void AssignStickFunctionality(ControllerInput _controllerInput)
    {
        leftStickPosition = new JoysticInput(_controllerInput.GetLeftStickPositions);
        rightStickPosition = new JoysticInput(_controllerInput.GetRightStickPositions);
    }
    public void ChangeAxisSpeed(byte _index, MOVEMENT_SPEED _initialSpeed)
    {
        // Changes the initial speed along the axis of choice
        switch (_initialSpeed)
        {
            case MOVEMENT_SPEED.JUMP:
                axisMovementSpeed[_index] = (sbyte)jumpStrength;
                break;
            case MOVEMENT_SPEED.RUN:
                axisMovementSpeed[_index] = (sbyte)runSpeed;
                break;
            case MOVEMENT_SPEED.WALK:
                axisMovementSpeed[_index] = (sbyte)walkSpeed;
                break;
        }
    }
    public void ChangeCharacterStates(byte _stateIndex, byte _subIndex)
    {
        // Changes character's state to whatever is being passed in
        currentState = availableStates[_stateIndex];
        currentState.ChangeCharacterSubState(_subIndex);
    }
    public void ControllerRequest(CONTROLLER_BUTTON _buttonPressed)
    {
        // If player is trying to move
        if (_buttonPressed == CONTROLLER_BUTTON.JOY_STICK_LEFT)
            currentState.Move();

        // If player is trying to look around
        if (_buttonPressed == CONTROLLER_BUTTON.JOY_STICK_RIGHT)
            RotateCharacter();

        // If player is pressing a button
        else
        {
            switch (_buttonPressed)
            {
                case CONTROLLER_BUTTON.CIRCLE:
                    break;
                case CONTROLLER_BUTTON.DPAD_LEFT:
                    break;
                case CONTROLLER_BUTTON.DPAD_UP:
                    break;
                case CONTROLLER_BUTTON.DPAD_RIGHT:
                    break;
                case CONTROLLER_BUTTON.DPAD_DOWN:
                    break;
                case CONTROLLER_BUTTON.L1:
                    break;
                case CONTROLLER_BUTTON.L2:
                    PullTrigger(1);
                    break;
                case CONTROLLER_BUTTON.L3:
                    currentState.ToggleRun();
                    break;
                case CONTROLLER_BUTTON.OPTIONS:
                    break;
                case CONTROLLER_BUTTON.R1:
                    break;
                case CONTROLLER_BUTTON.R2:
                    PullTrigger(2);
                    break;
                case CONTROLLER_BUTTON.R3:
                    break;
                case CONTROLLER_BUTTON.SHARE:
                    break;
                case CONTROLLER_BUTTON.SQUARE:
                    break;
                case CONTROLLER_BUTTON.TOUCH_PAD:
                    break;
                case CONTROLLER_BUTTON.TRIANGLE:
                    break;
                case CONTROLLER_BUTTON.X:
                    currentState.Jump();
                    break;
            }
        }
    }
    // Function looks weird, because up vector3.up and subtracting gravity, but it works: DO NOT CHANGE!
    public void Fall()
    {
        // Move character downward
        transform.Translate(Vector3.up * axisMovementSpeed[1] * Time.deltaTime);

        // Increase downward force over time
        axisMovementSpeed[1] -= (sbyte)gravityStrength;

        if (transform.position.y < characterStartingY)
            Land();
    }
    public CharacterStates GetCurrentState() { return currentState; }
    public CharacterStates GetRequestedState(byte _index) { return availableStates[_index]; }
    public void Jump()
    {
        // Move character upward
        transform.Translate(Vector3.up * axisMovementSpeed[1] * Time.deltaTime);

        // Reduce upward force over time
        axisMovementSpeed[1] -= (sbyte)gravityStrength;

        // If no more upward force, fall
        if (axisMovementSpeed[1] < 0)
            currentState.ChangeCharacterSubState(1);
    }
    public void MoveCharacter()
    {
        // Receiving information from joystick input
        joystickInformation[0] = leftStickPosition();

        // Transforming controller input into a direction
        moveInformation = transform.TransformDirection(joystickInformation[0][0], 0, joystickInformation[0][1]);

        // Applying speed to the direction
        moveInformation *= axisMovementSpeed[0];

        // Moving the character
        GetComponent<CharacterController>().Move(moveInformation * Time.deltaTime);
    }
    public void RotateCharacter()
    {
        // Receiving information from joystick input
        joystickInformation[1] = rightStickPosition();

        // Rotating the character around the world y-axis
        transform.Rotate(0, joystickInformation[1][0] * horizontalRotationSpeed * Time.deltaTime, 0);

        // Rotating the camera around the local x-axis
        verticalRotatorTransform.Rotate(joystickInformation[1][1] * verticalRotationSpeed * Time.deltaTime, 0, 0);
    }
    public void StorePreviousGroundingState(byte _state) { previousGroundedState = _state; }
    #endregion

    #region Blackbox
    private void Land()
    {
        // Updating character position
        landingPosition.x = transform.position.x;
        landingPosition.y = characterStartingY;
        landingPosition.z = transform.position.z;
        transform.position = landingPosition;

        // Updating character's state back to grounded
        ChangeCharacterStates(0, previousGroundedState);
    }
    private void PullTrigger(byte _index)
    {
        // Create a vertical rotator and attach the camera and guns to it
        //transform.GetChild(1).GetChild(_index).gameObject.GetComponent<WeaponProto>().Fire();
    }
    #endregion
}