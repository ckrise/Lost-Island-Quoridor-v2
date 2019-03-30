using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntBehavior : MonoBehaviour
{
    public static GruntBehavior Reference;
    private Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        Reference = this;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void RaiseWall()
    {
        animator.SetTrigger("raiseWall");
    }
    public void MovePawn()
    {
        animator.SetTrigger("movePawn");
    }
    public void Lose()
    {
        animator.SetTrigger("die");
    }
}
