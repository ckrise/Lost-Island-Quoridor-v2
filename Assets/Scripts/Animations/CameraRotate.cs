using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Transform target;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        // https://forum.unity.com/threads/rotate-the-camera-around-the-object.47353/
        transform.Translate(Vector3.left * Time.deltaTime * speed);
        transform.LookAt(target);
    }
}
