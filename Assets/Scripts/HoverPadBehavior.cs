using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPadBehavior : MonoBehaviour
{
    public void OnMouseEnter()
    {
        GUIController.GUIReference.ActivateGhostWall(
            gameObject.transform.position,
            gameObject.name[2]);
    }

    public void OnMouseExit()
    {
        GUIController.GUIReference.DeactivateGhostWall();
    }

    void OnMouseDown()
    {
        GUIController.GUIReference.PlacePlayerWall(
            gameObject.transform.position,
            gameObject.name);
    }
}
