using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBehavior : MonoBehaviour
{
    public static MageBehavior Reference;
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
        animator.SetTrigger("summon");
    }
    public void MovePawn()
    {
        animator.SetTrigger("attack");
    }   
}
