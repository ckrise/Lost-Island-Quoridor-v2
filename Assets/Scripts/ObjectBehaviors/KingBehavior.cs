using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBehavior : MonoBehaviour
{
    public static KingBehavior Reference;
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
        animator.SetTrigger("raise");
    }
    public void MovePawn()
    {
        animator.SetTrigger("attack1");
    }
    public void Lose()
    {
        animator.SetTrigger("die");
    }
}
