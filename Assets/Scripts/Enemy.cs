using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Movable {

    public int iDamage = 5;
    public int iSkipHit = 20;
    public int iSkipAmount = 5;
    public int iSkipMove = 5;
    public float fFollowTollerance = 0.25f;

    private Animator animator;
    private Transform target;
    private Vector2 vDirection;

    // Use this for initialization
    protected override void Start ()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        ResetSkipMove();
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        vDirection = new Vector2(xDir, yDir);
        ResetSkipMove();
        base.AttemptMove<Player>(xDir, yDir);
    }

    public void ResetSkipMove()
    {
        iSkipMove = iSkipAmount;
    }

    public void DecSkipMove()
    {
        iSkipMove--;
    }

    public void MoveEnemy()
    {
        int xDir = target.position.x > transform.position.x ? 1 : -1;
        int yDir = target.position.y > transform.position.y ? 1 : -1;

        if (!(target.position.y - fFollowTollerance > transform.position.y || target.position.y + fFollowTollerance < transform.position.y))
        {
            yDir = 0;
        }
        else if (!(target.position.x - fFollowTollerance > transform.position.x || target.position.x + fFollowTollerance < transform.position.x))
        {
            xDir = 0;
        }

        AnimCheck(xDir, yDir);

        AttemptMove<Player>(xDir, yDir);
    }

    //Check the animations for movement, setting the correct one based on inputs
    void AnimCheck(int xDir, int yDir)
    {
        if (xDir == 1)
        {
            animator.SetBool("playerRight", true);
        }
        else
        {
            animator.SetBool("playerRight", false);
            if (xDir == -1)
            {
                animator.SetBool("playerLeft", true);
            }
            else
            {
                animator.SetBool("playerLeft", false);
            }
        }

        if (yDir == 1)
        {
            animator.SetBool("playerUp", true);
        }
        else
        {
            animator.SetBool("playerUp", false);
            if (yDir == -1)
            {
                animator.SetBool("playerDown", true);
            }
            else
            {
                animator.SetBool("playerDown", false);
            }
        }
    }

    protected override void OnCantMove<T>(T component)
    {        
        Player hitPlayer = component as Player;

        hitPlayer.PushBack(vDirection);

        iSkipMove = iSkipHit;

        //animator.SetTrigger("enemyAttack");

        hitPlayer.LoseHP(iDamage);

        //SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}
