using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public Animator anime;
    public InputHandler inputHandler;
    public PlayerLocomotion playerLocomotion;
    private int vertical;
    private int horizontal;
    public bool canRotate;

    public void Initialzie()
    {
        anime = GetComponent<Animator>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimator(float ver, float hori, bool isSprinting)
    {
        float v = 0;
        if(ver > 0 && ver < 0.55f)
        {
            v = 0.5f;
        }
        else if(ver > 0.55f)
        {
            v = 1;
        }
        else if(ver < 0 && ver > -0.55f)
        {
            v = -0.5f;
        }
        else if(ver < -0.55f)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }

        float h = 0;
        if (hori > 0 && hori < 0.55f)
        {
            h = 0.5f;
        }
        else if (hori > 0.55f)
        {
            h = 1;
        }
        else if (hori < 0 && hori > -0.55f)
        {
            h = -0.5f;
        }
        else if (hori < -0.55f)
        {
            h = -1;
        }
        else
        {
            h = 0;
        }

        if(isSprinting)
        {
            v = 2;
            h = hori;
        }

        anime.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        anime.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    }

    public void PlayAnimation(string name, bool isInteracting)
    {
        anime.applyRootMotion = isInteracting;
        anime.SetBool("isInteracting", isInteracting);
        anime.CrossFade(name, 0.2f);
    }

    public void CanRotate()
    {
        canRotate = true;
    }

    public void StopRotation()
    {
        canRotate = false;
    }

    private void OnAnimatorMove()
    {
        if (!inputHandler.isInteracting)
            return;

        float delta = Time.deltaTime;
        playerLocomotion.rb.drag = 0;
        Vector3 deltaPos = anime.deltaPosition;
        deltaPos.y = 0;
        Vector3 velocity = deltaPos / delta;
        playerLocomotion.rb.velocity = velocity;

    }

}
