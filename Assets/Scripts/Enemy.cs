using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Movable {

    public int iDamage;
    public int iSkipAmount = 5;
    public int iSkipMove;
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
        base.AttemptMove<Player>(xDir, yDir);
        ResetSkipMove();
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
        int xDir = 0;
        int yDir = 0;

        yDir = target.position.y > transform.position.y ? 1 : -1;
        xDir = target.position.x > transform.position.x ? 1 : -1;

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {        
        Player hitPlayer = component as Player;

        hitPlayer.transform.position += new Vector3(vDirection.x, vDirection.y) * Time.deltaTime * pushBack;

        //animator.SetTrigger("enemyAttack");

        hitPlayer.LoseHP(iDamage);

        //SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}
