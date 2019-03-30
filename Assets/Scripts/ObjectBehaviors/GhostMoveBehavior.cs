using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMoveBehavior : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameData.IsTutorial)
        {
            TutorialController.Instance.MovePlayerPawn(gameObject);
        }
        else
        {
            GUIController.Instance.MovePlayerPawn(gameObject);
        }
        
    }  
}
