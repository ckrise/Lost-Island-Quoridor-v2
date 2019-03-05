using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnBehavior : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameData.IsTutorial)
        {
            TutorialController.Instance.ShowGhostMoves();
        }
        else
        {
            GUIController.Instance.ShowGhostMoves();
        }
    }
}
