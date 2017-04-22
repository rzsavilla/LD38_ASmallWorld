using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Movable {

    private Animator animator;
    private int iHP;
    private int iScore;
    private Vector3 vLastPosition;
    private Vector2 vDirection;

	// Use this for initialization
	protected override void Start ()
    {
        animator = GetComponent<Animator>();
        iHP = GameManager.instance.iPlayerHP;
        iScore = GameManager.instance.iScore;
        //Set da text

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

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Enemy>(horizontal, vertical);
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
        /*if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }*/
    }

    protected override void OnCantMove<T>(T component)
    {
        transform.position = vLastPosition - (new Vector3(vDirection.x, vDirection.y) * Time.deltaTime * pushBack);
        
        /*Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");*/
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseHP(int loss)
    {
        //animator.SetTrigger("playerHit");
        iHP -= loss;
        //foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (iHP <= 0)
        {
            //SoundManager.instance.PlaySingle(gameoverSound);
            //SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}
