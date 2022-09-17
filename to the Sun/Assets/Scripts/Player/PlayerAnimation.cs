using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Move(float input)
    {
        animator.SetFloat("Move", Mathf.Abs(input));
    }

    public void Slide(bool slide)
    {
        animator.SetBool("Slide", slide);
    }

    public void Attack(int attackNumber)
    {
        if (attackNumber == 1)
            animator.SetTrigger("Attack");
        if (attackNumber == 2)
            animator.SetTrigger("Second Attack");
    }

    public void TakeDamage()
    {
        animator.SetTrigger("Take Damage");
    }

    public void Death()
    {
        animator.SetTrigger("Death");
    }

    public void Defense(bool defense)
    {
        animator.SetBool("Defense", defense);
    }
}
