using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private Animator animator;
    private bool isAnimated = false;
    public bool IsFinishedAnimating { get => isAnimated; }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void AnimateCamera()
    {
        animator.SetTrigger("");
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
