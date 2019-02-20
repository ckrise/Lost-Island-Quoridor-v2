using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAnimation : MonoBehaviour
{
    private Vector3 destination;
    public float speed = 2;
    public float startdepth = 2;
    private bool isPlayer = false;

    // Update is called once per frame
    void Update()
    {
        if (destination != gameObject.transform.position)
        {
            IncrementPosition();
        }
    }


    void IncrementPosition()
    {
        float delta = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(
            transform.position, destination, delta);
        if (isPlayer && destination == gameObject.transform.position)
        {
            GUIController.GUIReference.AnimationCompleted();
        }
    }

    public void SetDestination(Vector3 value, bool isPlayer)
    {
        this.isPlayer = isPlayer;
        Vector3 start = value;
        start.y -= startdepth;
        transform.position = start;
        destination = value;
    }

}
