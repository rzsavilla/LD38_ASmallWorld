using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Movable {

    public bool bHurt = true;
    public int iHP = 10;
    public int iDamage = 5;
    public int iSkipHit = 20;
    public int iSkipAmount = 5;
    public int iSkipMove = 5;
    public int pushBack = 10;
    public float fFollowTollerance = 0.25f;
    public bool bDelete = false;

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
        base.AttemptMove<T>(xDir, yDir);
    }

    public void ResetSkipMove()
    {
        iSkipMove = iSkipAmount;
    }

    public void DecSkipMove()
    {
        iSkipMove--;
        if (iSkipMove <= 0)
        {
            bHurt = true;
        }
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
        
        //Render object with lowest y axis position
        GetComponent<SpriteRenderer>().sortingOrder = 1000 - (int)(GetComponent<Transform>().position.y * 100);
    }

    protected override void OnCantMove<T>(T component)
    {        
        Player hitPlayer = component as Player;

        if (hitPlayer.iState == 0)
        {

            hitPlayer.PushBack(vDirection);

            iSkipMove = iSkipHit;

            //animator.SetTrigger("enemyAttack");

            hitPlayer.LoseHP(iDamage);

            //SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
            animator.SetBool("playerAttack", true);
            animator.SetBool("playerUp", false);
            animator.SetBool("playerDown", false);
            animator.SetBool("playerLeft", false);
            animator.SetBool("playerRight", false);

        }
        else if (hitPlayer.iState == 1)
        {
            animator.SetBool("playerAttack", false);
            PushBack(-vDirection);
        }
    }

    //Function for being pushed back
    public void PushBack(Vector2 direction)
    {
        if (bHurt)
        {
            Vector2 position = transform.position;
            Vector2 newDirection = new Vector2(direction.x, direction.y) * Time.deltaTime * pushBack;

            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.enabled = false;
            RaycastHit2D hit = Physics2D.Linecast(position, position + newDirection, blockingLayer);
            boxCollider.enabled = true;

            if (hit.transform == null)
            {
                transform.position += (new Vector3(newDirection.x, newDirection.y));

                bHurt = false;
                iSkipMove = iSkipHit;
            }

            if (iHP <= 0)
            {
                bDelete = true;
            }
        }
    }

    //Function for being pushed back from the travelling player
    public void PushBackForce(Vector2 direction)
    {
        if (bHurt)
        {
            Vector2 position = transform.position;
            Vector2 newDirection = new Vector2(direction.x, direction.y) * Time.deltaTime * pushBack;

            transform.position += (new Vector3(newDirection.x, newDirection.y));

            bHurt = false;
            iSkipMove = iSkipHit;

            if (iHP <= 0)
            {
                bDelete = true;
            }
        }
    }

    public void Hit(int damage)
    {
        iHP -= damage;
    }
}
