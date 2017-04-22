using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Movable {

    public float restartLevelDelay = 1f;

    public Text tHP;
    public Text tScore;

    private Animator animator;
    private int iHP;
    private int iScore;
    private Vector3 vLastPosition;
    private Vector2 vDirection;

    //Score values for pickups
    private int iPickup1 = 5;
    private int iPickup2 = 10;
    private int iPickup3 = 20;

    // Use this for initialization
    protected override void Start ()
    {
        animator = GetComponent<Animator>();
        iHP = GameManager.instance.iPlayerHP;
        iScore = GameManager.instance.iScore;

        tHP.text = "HP: " + iHP;
        tScore.text = "Score: " + iScore;

        base.Start();
	}

    private void OnDisable()
    {
        GameManager.instance.iPlayerHP = iHP;
        GameManager.instance.iScore = iScore;
    }

    // Update is called once per frame
    void Update () {

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        //Will make it only move in 4 way not 8 way
        /*if (horizontal != 0)
        {
            vertical = 0;
        }*/

        AnimCheck(horizontal, vertical);

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Enemy>(horizontal, vertical);
        }
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

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        vDirection = new Vector2(xDir, yDir);
        vLastPosition = transform.position;
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            //SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        //CheckIfGameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Pickup1")
        {
            iScore += iPickup1;
            tScore.text = "Score: " + iScore;
            //SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Pickup2")
        {
            iScore += iPickup2;
            tScore.text = "Score: " + iScore;
            //SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Pickup3")
        {
            iScore += iPickup3;
            tScore.text = "Score: " + iScore;
            //SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        //PushBack();

        /*Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");*/
    }

    //Function for being pushed back when walking into an enemy
    public void PushBack()
    {
        transform.position = vLastPosition - (new Vector3(vDirection.x, vDirection.y) * Time.deltaTime * pushBack);
    }

    //Function for being pushed back from the enemy code
    public void PushBack(Vector2 direction)
    {
        transform.position += (new Vector3(direction.x, direction.y) * Time.deltaTime * pushBack);
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseHP(int loss)
    {
        //animator.SetTrigger("playerHit");
        iHP -= loss;
        tHP.text = tHP.text = "HP: " + iHP;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (iHP <= 0)
        {
            //SoundManager.instance.PlaySingle(gameoverSound);
            //SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
            DestroyObject(this);
        }
    }
}
