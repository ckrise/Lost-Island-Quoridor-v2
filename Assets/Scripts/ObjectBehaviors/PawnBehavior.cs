using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnBehavior : MonoBehaviour
{
    public void SetTransparent()
    {
        Color c = GetComponent<Renderer>().material.color;
        c.a = .5f;
        GetComponent<Renderer>().material.color = c;   
    }
    public void SetOpaque()
    {
        Color c = GetComponent<Renderer>().material.color;
        c.a = 1.0f;
        GetComponent<Renderer>().material.color = c;
    }
    private void OnMouseDown()
    {
        if (GameData.IsTutorial)
        {
            if (TutorialController.Instance.IsPlayerTurn())
            {
                TutorialController.Instance.ShowGhostMoves();
            }
        }
        else
        {
            GUIController.Instance.ShowGhostMoves();
        }
    }
}
