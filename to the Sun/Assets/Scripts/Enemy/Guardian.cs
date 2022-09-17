using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : Enemy, IDamageable
{
    public int currentHealth { get; set; }
    public bool hitDown;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        currentHealth = base.health;
    }

    protected override void Movement()
    {
        base.Movement();
        hitDownControl();
        
        Vector3 direction = player.transform.localPosition - transform.localPosition;

        if(direction.x > 0 && animator.GetBool("InCombat") == true)
        {
            sprite.flipX = false;
        }
        else if(direction.x < 0 && animator.GetBool("InCombat") == true)
        {
            sprite.flipX = true;
        }
    }

    public void hitDownControl()
    {
        if(player.successfullDefense)
        {
            animator.SetTrigger("HitDown");
            player.successfullDefense = false;
        }
    }

    public void Damage(int damageAmount)
    {
        if (currentHealth < 1)
        {
            return;
        }
        currentHealth--;
        FindObjectOfType<AudioManager>().Play("Hit");
        animator.SetTrigger("TakeDamage");
        isHit = true;
        animator.SetBool("InCombat", true);
        animator.SetBool("Idle", true);

        if(currentHealth < 1)
        {
            FindObjectOfType<AudioManager>().Play("Death");
            animator.SetTrigger("Death");
            player.numOfEnemy--;
            isDead = true;
        }
    }
}
