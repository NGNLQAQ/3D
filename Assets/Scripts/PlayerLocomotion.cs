using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    InputHandler input;
    public Vector3 direction;
    public Rigidbody rb;
    Vector2 movementInput;
    Transform cameraObject;
    Vector3 normal;
    Vector3 targetPos;

    [HideInInspector]
    public Transform playerTransform;
    [HideInInspector]
    public AnimatorHandler animator;

    [Header("Ground & Air Detection Stats")]
    [SerializeField]
    float groundDetectionStart = 0.5f;
    [SerializeField]
    float minDistanceforFalling = 1f;
    [SerializeField]
    float rayDistance = 0.2f;
    LayerMask ignoreGround;
    public float inAirTime;

    [Header("Stats")]
    [SerializeField]
    float movementSpeed = 5;
    [SerializeField]
    float sprintSpeed = 7;
    [SerializeField]
    float ratationSpeed = 10;
    [SerializeField]
    float fallingSpeed = 45;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
        input = GetComponent<InputHandler>();
        animator = GetComponentInChildren<AnimatorHandler>();
        cameraObject = Camera.main.transform;
        playerTransform = transform;

        animator.Initialzie();
        playerManager.isGrounded = true;
        ignoreGround = ~2;
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
            playerManager.isSprinting = true;
        }
        else
        {
            speed = movementSpeed;
            playerManager.isSprinting = false;
        }
                
        direction *= speed;

        Vector3 velocity = Vector3.ProjectOnPlane(direction, normal);
        rb.velocity = velocity;

        animator.UpdateAnimator(input.movement, 0, playerManager.isSprinting);

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

    public void Falling(float delta, Vector3 moveDirection)
    {
        playerManager.isGrounded = false;
        RaycastHit hit;
        Vector3 origin = playerTransform.position;
        origin.y += groundDetectionStart;

        if (Physics.Raycast(origin, playerTransform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }

        if(playerManager.isInAir)
        {
            rb.AddForce(-Vector3.up * fallingSpeed);
            rb.AddForce(moveDirection * fallingSpeed / 10f);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin += dir * rayDistance;

        targetPos = playerTransform.position;

        Debug.DrawRay(origin, -Vector3.up * minDistanceforFalling, Color.red, 0.1f, false);
        if(Physics.Raycast(origin, -Vector3.up, out hit, minDistanceforFalling, ignoreGround))
        {
            normal = hit.normal;
            Vector3 tp = hit.point;
            playerManager.isGrounded = true;
            targetPos.y = tp.y;

            if(playerManager.isInAir)
            {
                if(inAirTime > 0.5f)
                {
                    animator.PlayAnimation("Land", true);
                }
                else
                {
                    animator.PlayAnimation("Locomotion", false);
                }
                inAirTime = 0;
                playerManager.isInAir = false;
            }
        }
        else
        {
            if(playerManager.isGrounded)
            {
                playerManager.isGrounded = false;
            }

            if(!playerManager.isInAir)
            {
                if(!playerManager.isInteracting)
                {
                    animator.PlayAnimation("Falling", true);
                }

                Vector3 vel = rb.velocity;
                vel.Normalize();
                rb.velocity = vel * (movementSpeed / 2);
                playerManager.isInAir = true;
            }
        }

        if(playerManager.isGrounded)
        {
            if(playerManager.isInteracting || input.movement > 0)
            {
                playerTransform.position = Vector3.Lerp(playerTransform.position, targetPos, delta);
            }
            else
            {
                playerTransform.position = targetPos;
            }
        }
    }
}
