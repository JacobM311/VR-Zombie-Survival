using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAttachmentPointAnimation : MonoBehaviour
{
    public Animator animator;
    public string animationBoolName;

    public void ActivateAnimation()
    {
        if (animator != null && !string.IsNullOrEmpty(animationBoolName))
        {
            animator.SetBool(animationBoolName, true);
        }
    }

    public void DeactivateAnimation()
    {
        if (animator != null && !string.IsNullOrEmpty(animationBoolName))
        {
            animator.SetBool(animationBoolName, false);
        }
    }
}
