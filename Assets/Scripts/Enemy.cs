using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Movable {

    public int iDamage;
    public int iSkipAmount = 5;

    private int iSkipMove;
    private Animator animator;
    private Transform target;

    // Use this for initialization
    protected override void Start ()
    {
        //GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        iSkipMove = iSkipAmount;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (iSkipMove <= 0)
        {
            iSkipMove = iSkipAmount;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        iSkipMove--;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        
        Player hitPlayer = component as Player;

        //animator.SetTrigger("enemyAttack");

        hitPlayer.LoseHP(iDamage);

        //SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}
