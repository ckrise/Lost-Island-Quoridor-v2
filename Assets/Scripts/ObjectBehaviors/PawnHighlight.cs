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
    public void Hide()
    {
        GetComponent<Renderer>().enabled = false;
    }
    public void Show()
    {
        if (GameData.IsTutorial)
        {
            if (TutorialController.Instance.IsPlayerTurn())
            {
                GetComponent<Renderer>().enabled = true;
            }
        }
        else if (GUIController.Instance.IsPlayerTurn())
        {
            GetComponent<Renderer>().enabled = true;
        }
    }
}
