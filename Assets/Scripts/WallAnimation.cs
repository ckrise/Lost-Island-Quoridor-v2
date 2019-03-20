using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAnimation : MonoBehaviour
{
    private Vector3 destination;
    public float speed = 2;
    public float startdepth = 2;
    private bool isPlayer = false;
    private bool isAnimated = false;


    private void Awake()
    {       
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
       
        if (isAnimated && destination != transform.position)
        {
            IncrementPosition();
        }
    }


    void IncrementPosition()
    {
        float delta = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(
            transform.position, destination, delta);
        if (destination == transform.position)
        {
            if (GameData.IsTutorial)
            {
                TutorialController.Instance.AnimationCompleted(isPlayer);
            }
            else
            {
                GUIController.Instance.AnimationCompleted(isPlayer);
            }
        }
    }

    public void Animate(Vector3 value, bool isPlayer)
    {
        isAnimated = true;
        this.isPlayer = isPlayer;
        Vector3 start = value;
        start.y -= startdepth;
        transform.position = start;
        destination = value;
    }
    public void RemoveWallFromPool()
    {
        isAnimated = true;
        Vector3 end = transform.position;
        end.y -= startdepth;
        destination = end;
    }
    public void AddWallToPool()
    {
        isAnimated = false;
        Vector3 newPosition = transform.position;
        newPosition.y = .05f;
        transform.position = newPosition;
    }
}