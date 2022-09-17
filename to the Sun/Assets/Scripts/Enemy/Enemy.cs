using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int health;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected Transform startPoint, endPoint;

    protected Vector3 target;
    protected Animator animator;
    protected SpriteRenderer sprite;

    protected int i = 0;
    protected bool oldSprite;
    protected bool wait = false;
    protected float waitTime = 0;
    [SerializeField]
    protected float maxWaitTime;

    protected bool isHit = false;
    protected bool isDead = false;
    protected PlayerControl player;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        target = endPoint.position;
        player.numOfEnemy++;
    }

    protected virtual void Update()
    {
        if(isDead == false)
            Movement();
    }

    protected virtual void Movement()
    {
        if(i>=1)
        {
            sprite.flipX = oldSprite;
            i=0;
        }
        if (transform.position == startPoint.position && !wait && !isHit)
        {
            target = endPoint.position;
            wait = true;
            sprite.flipX = true;
        }
        else if (transform.position == endPoint.position && !wait && !isHit)
        {
            target = startPoint.position;
            wait = true;
            sprite.flipX = false;
        }

        if (wait == false && isHit == false)
        {
            animator.SetBool("Idle", false);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            waitTime = 0;
        }
        else if(wait == true && isHit == false)
        {
            animator.SetBool("Idle", true);
            Wait();
        }
        else if(isHit == true)
        {
            i++;
            oldSprite = sprite.flipX;
            animator.SetBool("Idle", true);
        }

        float distance = Vector3.Distance(transform.localPosition, player.transform.localPosition);
        if(distance > 8.0f)
        {
            isHit = false;
            animator.SetBool("InCombat", false);
        }
    }

    protected virtual void Wait()
    {
        waitTime += Time.deltaTime;
        if (waitTime > maxWaitTime)
        {
            wait = false;
        }
    }
}
