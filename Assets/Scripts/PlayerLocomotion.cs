using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputHandler input;
    Vector3 direction;
    public Rigidbody rb;
    Vector2 movementInput;
    Transform cameraObject;

    [HideInInspector]
    public Transform playerTransform;
    [HideInInspector]
    public AnimatorHandler animator;

    [Header("Stats")]
    [SerializeField]
    float movementSpeed = 5;
    [SerializeField]
    float sprintSpeed = 7;
    [SerializeField]
    float ratationSpeed = 10;

    public bool isSprinting;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<InputHandler>();
        animator = GetComponentInChildren<AnimatorHandler>();
        cameraObject = Camera.main.transform;
        playerTransform = transform;

        animator.Initialzie();
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        input.PlayerInput(delta);
        Moving(delta);
        RollingAndSprinting(delta);
    }

    public void Moving(float delta)
    {
        direction = cameraObject.forward * input.vertical;
        direction += cameraObject.right * input.horizontal;
        direction.Normalize();
        direction.y = 0;

        float speed;
        if(input.sprint)
        {
            speed = sprintSpeed;
            isSprinting = true;
        }
        else
        {
            speed = movementSpeed;
            isSprinting = false;
        }
                
        direction *= speed;

        Vector3 velocity = Vector3.ProjectOnPlane(direction, new Vector3(0, 0, 0));
        rb.velocity = velocity;

        animator.UpdateAnimator(input.movement, 0, isSprinting);

        if (animator.canRotate)
        {
            Rotation(delta);
        }
    }

    private void Rotation(float delta)
    {
        Vector3 target = Vector3.zero;

        target = cameraObject.forward * input.vertical;
        target += cameraObject.right * input.horizontal;
        target.Normalize();
        target.y = 0;

        if(target == Vector3.zero)
        {
            target = playerTransform.forward;
        }

        Quaternion rotation = Quaternion.LookRotation(target);
        Quaternion targetRotation = Quaternion.Slerp(playerTransform.rotation, rotation, ratationSpeed * delta);

        playerTransform.rotation = targetRotation;
    }
    public void RollingAndSprinting(float delta)
    {
        if (animator.anime.GetBool("isInteracting"))
            return;

        if(input.roll)
        {
            direction = cameraObject.forward * input.vertical;
            direction += cameraObject.right * input.horizontal;

            if(input.movement > 0)
            {
                animator.PlayAnimation("Roll", true);
                direction.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(direction);
                playerTransform.rotation = rollRotation;
            }
            else
            {
                animator.PlayAnimation("Backstep", true);
            }
        }
    }
}
