using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : GameObjectBase
{
    #region Variables
    // Inspecter assigned variables
    [SerializeField] private AudioClip[] audioClips;                       // 0 - Footsteps. 1 - Dash
    [SerializeField] private byte dashRange;
    [SerializeField] private byte dashSpeed;
    [SerializeField] private byte initialNumberOfBombs;
    [SerializeField] private byte initialNumberOfLives;
    [SerializeField] private byte walkSpeed;
    [SerializeField] private byte horizontalRotationSpeed;
    [SerializeField] private byte verticalRotationSpeed;
    [SerializeField] private ushort bombRadius;
    [SerializeField] private ushort dashCoolingTime;

    private AudioSource movementAudio;
    private bool dashCooling;
    private byte currentNumberOfBombs;
    private byte currentNumberOfLives;
    private byte moveSpeed;
    private byte prePauseState;

    // State Machine
    private CharacterStates[] availableStates;                             // 0 - Idle. 1 - Waling. 2 - Dashing
    private CharacterStates currentState;

    private ControllerInput_GetStickPositions GetLeftStickPositions;       // Function pointer to receive joystick direction information
    private ControllerInput_GetStickPositions GetRightStickPositions;      // Function pointer to receive joystick direction information
    private delegate float[] ControllerInput_GetStickPositions();
    private delegate void GameController_TogglePause();
    private float currentDashDistance;
    private float[][] joystickInformation;                                 // Storage location for joystick information
    private GameController_TogglePause TogglePause;
    private GameObject bombExplosion;
    private int score;
    private long timeStartedCooling;
    private MeshRenderer[] dashIndicators;
    private Transform verticalRotatorTransform;                            // Look up and down
    private Vector3 dashDirection;
    private Vector3 dashStartPosition;
    private Vector3 moveInformation;                                       // The direction and speed of character movement
    private WeaponManager[] weaponManagers;                                // The guns that the player is holding
    #endregion

    #region Initialization
    private void Awake()
    { 
        movementAudio = GetComponent<AudioSource>();
        dashCooling = false;
        currentNumberOfBombs = initialNumberOfBombs;
        currentNumberOfLives = initialNumberOfLives;
        currentDashDistance = 0;
        moveSpeed = walkSpeed;
        
        // Setting up joystick information storage
        joystickInformation = new float[2][];
        joystickInformation[0] = new float[2] { 0, 0 };
        joystickInformation[1] = new float[2] { 0, 0 };

        // Initializing states and setting character to grounded
        availableStates = new CharacterStates[] { new Idle(this), new Walking(this), new Dashing(this), new CharacterPaused(this) };
        currentState = availableStates[0];

        dashIndicators = new MeshRenderer[2] { transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<MeshRenderer>(), transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<MeshRenderer>() };
        verticalRotatorTransform = transform.GetChild(0).transform;

        // Bad, do better
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
        weaponManagers = new WeaponManager[] { weapons[0].GetComponent<WeaponManager>(), weapons[1].GetComponent<WeaponManager>() };
    }
    public void Start()
    {
        Cacher.billBoardManager.UpdateBillBoardText(0, currentNumberOfBombs);
        Cacher.billBoardManager.UpdateBillBoardText(1, currentNumberOfLives);

        // Find a better way to do this. Hacked to allow the first weapon change to privide the right number of rounds and the hud to display it
        for (byte index = 0; index < 2; ++index)
        {
            // Updating the HUD to reflect number of bullets in weapons
            Cacher.hudManager.UpdateRoundCount(index, weaponManagers[index].GetCurrentMagCount());

            // Adding weapons to pauseable gameobjects, so their updates will be called when needed
            Cacher.pauseableGameObjects.Add(weaponManagers[index]);
        }
    }
    #endregion

    #region Main Update
    // Called by gamecontroller
    public override void GameUpdate() { currentState.Update(); }
    
    // Called by attached states
    public void StateUpdate()
    {
        if (dashCooling)
        {
            if (Cacher.gameTime.ElapsedMilliseconds - timeStartedCooling > dashCoolingTime)
            {
                dashCooling = false;

                // Lets player know that dash is available
                dashIndicators[0].material.color = Color.green;
                dashIndicators[1].material.color = Color.green;
            }
        }
    }
    #endregion

    #region Public Interface
    public void AssignPauseDelegate(GameController _gameController) { TogglePause = _gameController.TogglePause; }
    public void AssignStickFunctionality(ControllerInput _controllerInput)
    {
        GetLeftStickPositions = _controllerInput.GetLeftStickPositions;
        GetRightStickPositions = _controllerInput.GetRightStickPositions;
    }
    public void ChangeAudioClips(byte _index)
    {
        switch (_index)
        {
            case 1:
                movementAudio.clip = audioClips[0];
                break;
            case 2:
                movementAudio.clip = audioClips[1];
                break;
            default:
                break;
        }
    }
    public void ChangeCharacterStates(byte _index)
    {
        currentState.Disable();

        // Changes character's state to whatever is being passed in
        currentState = availableStates[_index];

        ChangeAudioClips(_index);
        currentState.Enable();
    }
    public bool CheckState(byte _index)
    {
        if (currentState == availableStates[_index])
            return true;

        return false;
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
                case CONTROLLER_BUTTON.BOTTOM_FACE:
                    if (currentState == availableStates[1])
                    {
                        if (DashAvailability())

                            // Change character state to dashing
                            ChangeCharacterStates(2);
                    }
                    break;
                case CONTROLLER_BUTTON.DPAD_LEFT:
                    break;
                case CONTROLLER_BUTTON.DPAD_UP:
                    break;
                case CONTROLLER_BUTTON.DPAD_RIGHT:
                    break;
                case CONTROLLER_BUTTON.DPAD_DOWN:
                    break;
                case CONTROLLER_BUTTON.L1B:
                    break;
                case CONTROLLER_BUTTON.L2T:
                    PullTrigger(0);
                    break;
                case CONTROLLER_BUTTON.L3:
                    break;
                case CONTROLLER_BUTTON.LEFT_FACE:
                    ReloadWeapon(0);
                    break;
                case CONTROLLER_BUTTON.OPTIONS:
                    break;
                case CONTROLLER_BUTTON.R1B:
                    break;
                case CONTROLLER_BUTTON.R2T:
                    PullTrigger(1);
                    break;
                case CONTROLLER_BUTTON.R3:
                    ToggleZoom();
                    break;
                case CONTROLLER_BUTTON.RIGHT_FACE:
                    ReloadWeapon(1);
                    break;
                case CONTROLLER_BUTTON.SHARE:
                    break;
                case CONTROLLER_BUTTON.TOP_FACE:
                    UseBomb();
                    break;
                case CONTROLLER_BUTTON.TOUCH_PAD:
                    break;
            }
        }
    }
    // Needs to be given some kind of meter, so the player won't spam it so much
    public void Dash()
    {
        currentDashDistance = Vector3.Distance(transform.position, dashStartPosition);

        if (currentDashDistance < dashRange)
            // Moving the character in the last direction they were pressing
            GetComponent<CharacterController>().Move(dashDirection * moveSpeed * Time.deltaTime);

        else
        {
            currentDashDistance = 0;
            ChangeCharacterStates(1);
        }
    }
    public bool DashAvailability()
    {
        if (!dashCooling)
        {
            // Let player know that character cannot dash until green
            dashIndicators[0].material.color = Color.red;
            dashIndicators[1].material.color = Color.red;

            //Do better
            // Receiving information from joystick input
            joystickInformation[0] = GetLeftStickPositions();

            // Transforming controller input into a direction
            dashDirection = transform.TransformDirection(joystickInformation[0][0], 0, joystickInformation[0][1]);

            // Store character's current position
            dashStartPosition = transform.position;

            return true;
        }

        return false;
    }
    public byte GetCurrentNumberOfBombs() { return currentNumberOfBombs; }
    public byte GetCurrentNumberOfLives() { return currentNumberOfLives; }
    public CharacterStates GetCurrentState() { return currentState; }
    public CharacterStates GetRequestedState(byte _index) { return availableStates[_index]; }
    public int GetScore() { return score; }
    public void MoveCharacter()
    {
        // Receiving information from joystick input
        joystickInformation[0] = GetLeftStickPositions();

        // Transforming controller input into a direction
        moveInformation = transform.TransformDirection(joystickInformation[0][0], 0, joystickInformation[0][1]);

        // Moving the character
        GetComponent<CharacterController>().Move(moveInformation * moveSpeed * Time.deltaTime);        
    }
    public void PauseCharacter()
    {
        for (byte index = 0; index < 3; ++index)
        {
            // Save character's current state
            if (currentState == availableStates[index])
                prePauseState = index;
        }

        // Pause character
        ChangeCharacterStates(3);
    }
    public void PauseMovementAudio() { if (movementAudio.isPlaying) movementAudio.Pause(); }
    public void RotateCharacter()
    {
        // Receiving information from joystick input
        joystickInformation[1] = GetRightStickPositions();

        // Rotating the character around the world y-axis
        transform.Rotate(0, joystickInformation[1][0] * horizontalRotationSpeed * Time.deltaTime, 0);

        // Rotating the camera around the local x-axis
        verticalRotatorTransform.Rotate(joystickInformation[1][1] * verticalRotationSpeed * Time.deltaTime, 0, 0);
    }
    public void SetMoveSpeed(bool _walking) { moveSpeed = (_walking) ? walkSpeed : dashSpeed; }
    public void StartCooling() { dashCooling = true; }
    public void StartMovementAudio() { movementAudio.Play(); }
    public void StartTimeStartedCooling() { timeStartedCooling = Cacher.gameTime.ElapsedMilliseconds; }
    public void StopMovementAudio() { movementAudio.Stop(); }
    public void UnpauseCharacter() { ChangeCharacterStates(prePauseState); }
    public void UnpauseMovementAudio() { if (!movementAudio.isPlaying) movementAudio.UnPause(); }
    #endregion

    #region Blackbox
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (currentNumberOfLives > 0)
            {
                --currentNumberOfLives;
                Cacher.billBoardManager.UpdateBillBoardText(1, currentNumberOfLives);
                Cacher.PlayerDeath();
            }
            else
                SceneManager.LoadScene("Prototype");
        }
    }
    private void PullTrigger(byte _index)
    {
        // Fires weapon according to trigger pressed
        weaponManagers[_index].Fire(_index);
    }
    private void ReloadWeapon(byte _index) { weaponManagers[_index].ReplenishRounds(_index); }
    private void ToggleZoom()
    {
        Cacher.hudManager.ToggleTextElementVisibility();
        Camera.main.fieldOfView = (Camera.main.fieldOfView == 60) ? 20 : 60;
        Cacher.hudManager.ToggleReticuleSize();
    }
    private void UseBomb()
    {
        if (bombExplosion == null || !bombExplosion.activeInHierarchy)
        {
            if (currentNumberOfBombs > 0)
            {
                // Find an explosion that's not being used
                bombExplosion = Spawn.ReturnSpawnableAsset(Cacher.pooledAssets[1]);

                // Set positions, start line's life time, and turn it on
                bombExplosion.GetComponent<ExplosionBehavior>().SetAttributes(bombRadius, transform.position);
                bombExplosion.SetActive(true);

                --currentNumberOfBombs;

                // Update billboard
                Cacher.billBoardManager.UpdateBillBoardText(0, currentNumberOfBombs);
            }
        }
    }
    #endregion
}