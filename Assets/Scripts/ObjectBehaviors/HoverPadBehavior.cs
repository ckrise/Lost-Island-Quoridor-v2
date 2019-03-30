using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPadBehavior : MonoBehaviour
{
    public void OnMouseEnter()
    {
        if (GameData.IsTutorial)
        {
            TutorialController.Instance.ActivateGhostWall(
            transform.position,
            name[2]);
        }
        else
        {
            GUIController.Instance.ActivateGhostWall(
            transform.position,
            name[2]);
        }
    }

    public void OnMouseExit()
    {
        if (GameData.IsTutorial)
        {
            TutorialController.Instance.DeactivateGhostWall();
        }
        else
        {
            GUIController.Instance.DeactivateGhostWall();
        }
    }

    void OnMouseDown()
    {

        if (GameData.IsTutorial)
        {
            TutorialController.Instance.PlacePlayerWall(
                transform.position,
                name);
        }
        else
        {
            GUIController.Instance.PlacePlayerWall(
                transform.position,
                name);
        }
    }
}
