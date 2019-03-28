using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocBehavior : MonoBehaviour
{
    public static CrocBehavior Reference;
    private Animator animator;
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
        animator.SetTrigger("thumbsDown");
    }
    public void MovePawn()
    {
        animator.SetTrigger("think");
    }

}
