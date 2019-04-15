using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnimation : MonoBehaviour
{
    public float minLevel = -9.75f;
    public float maxLevel = -3f;
    public float increment = .025f;
    private Vector3 upIncrement, downIncrement;
    private bool rise = true;
    private int pause = 0;


    // Start is called before the first frame update
    void Start()
    {
        upIncrement = new Vector3(0, increment, 0);
        downIncrement = new Vector3(0, 0 - increment, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pause == 0)
        {
            if (rise)
            {
                transform.Translate(upIncrement * Time.deltaTime);
            }
            else
            {
                transform.Translate(downIncrement * Time.deltaTime);
            }
            if (transform.position.y >= maxLevel ||
                transform.position.y <= minLevel)
            {
                rise = !rise;
                pause = 10;
            }
        }
        else
        {
            pause--;
        }
    }
}
