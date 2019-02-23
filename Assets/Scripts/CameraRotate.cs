using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Transform target;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // https://forum.unity.com/threads/rotate-the-camera-around-the-object.47353/
        transform.Translate(Vector3.right * Time.deltaTime * speed);
        transform.LookAt(target);
    }
}
