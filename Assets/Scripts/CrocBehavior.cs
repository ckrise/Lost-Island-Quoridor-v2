using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocBehavior : MonoBehaviour
{
    public static CrocBehavior Reference;
    private Animation animator;
    // Start is called before the first frame update
    void Awake()
    {
        Reference = this;
    }
    void Start()
    {
        animator = GetComponent<Animation>();
    }

    public void RaiseWall()
    {
        animator.Play("Jump Up Crocodile");
        animator.Play("Idle Crocodile");
    }
    public void MovePawn()
    {
        animator.Play("Success Crocodile");
        animator.Play("Idle Crocodile");
    }
}
