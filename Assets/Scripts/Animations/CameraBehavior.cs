using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private bool isAnimated = false;
    public bool IsAnimated { get => isAnimated; }

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
