using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float movement;
    public float mouseX;
    public float mouseY;

    public bool b_Input;
    public bool roll;
    public bool sprint;
    public float rollTimer;
    public bool isInteracting;


    PlayerController input;
    CameraHandler cameraHandler;
    Vector2 movementInput;
    Vector2 cameraInput;
    private void Awake()
    {
        cameraHandler = CameraHandler.singleton;
    }
    private void FixedUpdate()
    {
        float delta = Time.deltaTime;
        
        if(cameraHandler != null)
        {
            cameraHandler.Follow(delta);
            cameraHandler.Rotation(delta, mouseX, mouseY);
        }
    }
    public void OnEnable()
    {
        if (input == null)
        {
            input = new PlayerController();
            input.PlayerMovement.MovementAction.performed += moveOutput => movementInput = moveOutput.ReadValue<Vector2>();
            input.PlayerMovement.Camera.performed += cameraOutput => cameraInput = cameraOutput.ReadValue<Vector2>();
        }

        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public void PlayerInput(float delta)
    {
        MoveInput(delta);
        Rolling(delta);
    }

    public void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        movement = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void Rolling(float delta)
    {
        b_Input = input.PlayerAction.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;

        if(b_Input)
        {
            rollTimer += delta;
            sprint = true;
        }
        else
        {
            if(rollTimer > 0 && rollTimer < 0.5f)
            {
                sprint = false;
                roll = true;
            }

            rollTimer = 0;
        }
    }
}
