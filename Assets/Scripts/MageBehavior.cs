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

    public void Summon()
    {
        animator.SetTrigger("summon");
    }
    public void Attack()
    {
        animator.SetTrigger("attack");
    }   
}
