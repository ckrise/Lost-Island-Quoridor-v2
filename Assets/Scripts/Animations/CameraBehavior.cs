using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public static CameraBehavior reference;
    private Animator animator;
    private bool isAnimated = false;
    public bool IsFinishedAnimating { get => isAnimated; }

    void Start()
    {
        reference = this;
        animator = GetComponent<Animator>();
    }

    public void AnimateCamera()
    {
        animator.SetTrigger("Enter");
    }

    public void AnimationFinished()
    {
        isAnimated = true;
        if (GameData.IsTutorial)
        {
            TutorialController.Instance.animationFinished = true;
        }
        else
        {
            GUIController.Instance.animationFinished = true;
        }
    }
}
