using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public void AnimationFinished()
    {
        GUIController.GUIReference.animationFinished = true;
    }
}
