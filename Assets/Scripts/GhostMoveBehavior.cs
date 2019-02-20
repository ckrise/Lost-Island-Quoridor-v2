using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMoveBehavior : MonoBehaviour
{    private void OnMouseDown()
    {
        GUIController.GUIReference.MovePlayerPawn(gameObject);
    }  
}
