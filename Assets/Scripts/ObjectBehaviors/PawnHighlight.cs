using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnHighlight : MonoBehaviour
{
    private void OnMouseOver()
    {
        Show();
    }
    private void OnMouseExit()
    {
        Hide();
    }
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
    private void Hide()
    {
        GetComponent<Renderer>().enabled = false;
    }
    private void Show()
    {
        if (GameData.IsTutorial && TutorialController.Instance.IsPlayerTurn() || GUIController.Instance.IsPlayerTurn())
        {
            GetComponent<Renderer>().enabled = true;
        }
    }
}
