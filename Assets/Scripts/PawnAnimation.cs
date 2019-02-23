using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAnimation : MonoBehaviour
{
    private Vector3 destination;
    public float speed = 2;
    private bool isPlayer = false;

    // http://answers.unity.com/answers/1426132/view.html
    // Start is called before the first frame update
    void Start()
    {
        destination = transform.position;
    }

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
        if (destination == gameObject.transform.position)
        {
            GUIController.GUIReference.AnimationCompleted(isPlayer);
        }
    }
    
    public void SetDestination(Vector3 value, bool isPlayer)
    {
        this.isPlayer = isPlayer;
        destination = value;
    }




}
