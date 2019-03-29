﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnBehavior : MonoBehaviour
{
    private void Start()
    {
        
    }
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
}
