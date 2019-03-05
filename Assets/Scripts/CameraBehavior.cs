using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public void AnimationFinished()
    {
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
