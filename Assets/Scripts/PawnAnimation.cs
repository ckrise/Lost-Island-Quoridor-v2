using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAnimation : MonoBehaviour
{
    private Vector3 destination;
    private float speed = 10;
    private bool isPlayer = false;
    private Vector3 startPosition;
    private float totalDistance;
    private float halfway;
    private int b;
    public AudioSource pawnMovementSound;

    // http://answers.unity.com/answers/1426132/view.html
    // Start is called before the first frame update
    void Start()
    {
        pawnMovementSound = GetComponent<AudioSource>();
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (destination != gameObject.transform.position)
        {
            IncrementPosition();
            AnimateY();
        }
    }

    void IncrementPosition()
    {
        float delta = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(
            transform.position, destination, delta);
        
        if (destination == gameObject.transform.position)
        {
            if (GameData.IsTutorial)
            {
                TutorialController.Instance.AnimationCompleted(isPlayer);
            }
            else
            {
                pawnMovementSound.Play();
                Debug.Log("Played Sound");
                GUIController.Instance.AnimationCompleted(isPlayer);
            }
        }
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        destination = newPosition;
    }

    public void Animate(Vector3 value, bool isPlayer)
    {
        
        this.isPlayer = isPlayer;
        destination = value;
        startPosition = transform.position;
        totalDistance = DistanceFormulaXZ(transform.position, destination);
        halfway = totalDistance / 2;
        b = CalculateB();
    }

    private void AnimateY()
    {
        var p = transform.position;
        float height = CalculateHeight();
        transform.position = new Vector3(p.x, height, p.z);

    }
    private int CalculateB()
    {
        int b;
        if (halfway == 1)
        {
            b = 1;
        }
        else if (halfway == 2)
        {
            b = 4;
        }
        else /*if (halfway == Mathf.Sqrt(8) / 2)*/
        {
            b = 2;
        }
        return b;
    }
    private float CalculateHeight()
    {
        float currentDist = DistanceFormulaXZ(transform.position, destination);
        float currentDistFromHalf = currentDist - halfway;
        return b - Mathf.Pow(currentDistFromHalf, 2);
    }

    private float DistanceFormulaXZ(Vector3 start, Vector3 end)
    {
        float xDiff = end.x - start.x;
        float zDiff = end.z - start.z;
        return Mathf.Sqrt(Mathf.Pow(xDiff, 2) + Mathf.Pow(zDiff, 2));
    }

}