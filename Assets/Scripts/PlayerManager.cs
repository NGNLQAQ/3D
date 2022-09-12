using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;

    [Header("Player Flag")]
    public bool isInteracting;
    public bool isSprinting;
    public bool isInAir;
    public bool isGrounded;

    private void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
    }

    private void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        isInteracting = anim.GetBool("isInteracting");

        inputHandler.PlayerInput(delta);
        playerLocomotion.Moving(delta);
        playerLocomotion.RollingAndSprinting(delta);
        playerLocomotion.Falling(delta, playerLocomotion.direction);
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime;

        if (cameraHandler != null)
        {
            cameraHandler.Follow(delta);
            cameraHandler.Rotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }
    }

    private void LateUpdate()
    {
        inputHandler.roll = false;
        inputHandler.sprint = false;

        if(isInAir)
        {
            playerLocomotion.inAirTime += Time.deltaTime;
        }
    }
}
