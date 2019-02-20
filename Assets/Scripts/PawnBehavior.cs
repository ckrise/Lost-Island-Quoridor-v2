using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnBehavior : MonoBehaviour
{
    private void OnMouseDown()
    {
        GUIController.GUIReference.ShowGhostMoves();
    }
}
