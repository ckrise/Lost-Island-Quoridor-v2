﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAnimation : MonoBehaviour
{
    private Vector3 destination;
    public float speed = 2;
    public float startdepth = 2;
    private bool isPlayer = false;


    private void Awake()
    {
       
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
       
        if (destination != transform.position)
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
            GUIController.GUIReference.AnimationCompleted(isPlayer);
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
    public void RemoveWall()
    {
        Vector3 end = transform.position;
        end.y -= startdepth;
        destination = end;
    }
}