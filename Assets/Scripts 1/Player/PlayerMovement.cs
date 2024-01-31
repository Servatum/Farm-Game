using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    //Objects
    public Camera playerCam;
    public Animator Legs;

    //Floats
    public float crouchSpeed = 2f;
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float jumpHeight = 3;
    public float gravity = 15;
    public float lookSpeed = 2f;
    float rotationX = 0;

    public float standingHeight = 2;
    public float crouchingHeight = 0.8f;

    //Bools
    public bool canMove = true;
    public bool crouching = false;
    public bool unCrouching = false;
    public bool climbing = false;

    Vector3 moveDirection = Vector3.zero;
    RaycastHit hit;

    CharacterController characterController;
    CapsuleCollider capsuleCollider;

    #endregion
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        capsuleCollider.enabled = false;
    }

    void Update()
    {
        switch (characterController.isGrounded)
        {
            case true:
                Legs.SetBool("IsGrounded", true);
                break;
            case false:
                Legs.SetBool("IsGrounded", false);
                break;
        }
        float movementDirectionY = moveDirection.y;
        #region Walk/Run
        if (climbing == false)
        {
            UpdateWalking();
        }

        #endregion

        #region Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpHeight;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Rotation
        characterController.Move(moveDirection * Time.deltaTime);


        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -90, 65);
        playerCam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        #endregion

        #region Crouch
        UpdateCrouching();
        #endregion

        #region Climbing
        if (climbing == true)
        {
            Updateclimbing();
        }
        #endregion
    }

    #region Triggers
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Ladder":
                climbing = true;
                gravity = 0;
                Debug.Log("LadderStuffHere");
                break;
            case "Slope":
                capsuleCollider.enabled = true;
                characterController.enabled = false;
                break;
            case "AddDrums":
                Debug.Log("Removed (AddDrums)");
                MusicDynamics.playingDrums = true;
                other.enabled = false;
                break;
            case "RemoveDrums":
                Debug.Log("Removed (RemoveDrums)");
                other.enabled = false;
                break;
            case "AddBass":
                Debug.Log("Removed (AddBass)");
                MusicDynamics.playingBass = true;
                other.enabled = false;
                break;
            case "RemoveBass":
                Debug.Log("Removed (RemoveBass)");
                other.enabled = false;
                break;
            case "RemoveAll":
                Debug.Log("Removed (RemoveAll)");
                MusicDynamics.disableAll = true;
                other.enabled = false;
                break;
            case "InstrumentVol":
                Debug.Log("LowerInstrumentVolume");
                MusicDynamics.musicVolume = false;
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        switch(other.tag)
        {
            case "Ladder":
                climbing = false;
                gravity = 15;
                break;
            case "Slope":
                StartCoroutine(SlopeTimer());
                break;
            case "InstrumentVol":
                MusicDynamics.musicVolume = true;
                break;
        }
    }
    #endregion

    #region Timers
    //Makes you get up smooth
    IEnumerator UncrouchTimer()
    {
        while (characterController.height < standingHeight)
        {
            yield return new WaitForSeconds(0.01f);
            characterController.height += 0.25f;
        }

        Legs.SetBool("IsCrouched", false);
        characterController.height = standingHeight;
        unCrouching = false;
    }

    //Adds a cooldown before you can move after sliding on a slope
    IEnumerator SlopeTimer()
    {
        yield return new WaitForSeconds(0.15f);
        characterController.enabled = true;
        capsuleCollider.enabled = false;
    }
    #endregion

    #region Functions
    void UpdateWalking()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (!crouching ? (isRunning ? runSpeed : walkSpeed) : crouchSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (!crouching ? (isRunning ? runSpeed : walkSpeed) : crouchSpeed) * Input.GetAxis("Horizontal") : 0;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        Legs.SetFloat("WalkF/B", curSpeedX);
        Legs.SetFloat("WalkL/R", curSpeedY);
    }
    void UpdateCrouching()
    {
        if (Input.GetKey(KeyCode.LeftControl) && canMove == true)
        {
            crouching = true;
            Legs.SetBool("IsCrouched", true);
            characterController.height = crouchingHeight;
        }
        else if (characterController.height != standingHeight && unCrouching == false)
        {
            crouching = false;
            unCrouching = true;
            StartCoroutine(UncrouchTimer());
        }
    }
    void Updateclimbing()
    {
        Vector3 up = transform.TransformDirection(Vector3.up);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float curSpeedX = Input.GetAxis("Vertical");
        float curSpeedY = Input.GetAxis("Horizontal");
        moveDirection = (up * curSpeedX * 2) + (right * curSpeedY * 2);
    }
    #endregion
}