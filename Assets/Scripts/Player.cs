using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Movable {

    public float restartLevelDelay = 1f;
    public float pushBack = 10f;
    public Text tHP;
    public Text tScore;
    public Text tCard;
    public int iCurrentCard;
    public int iSkipMove = 0;

    private Animator animator;
    private int iHP;
    private int iMaxHP;
    private int iScore;
    private Vector2 vDirection;
    private List<Card> cards;

    //Score values for pickups
    private int iPickup1 = 5;
    private int iPickup2 = 10;
    private int iPickup3 = 20;

    // Use this for initialization
    protected override void Start ()
    {
        animator = GetComponent<Animator>();
        cards = GameManager.instance.GetCards();
        iCurrentCard = GameManager.instance.iCurrentCard;
        iHP = GameManager.instance.iPlayerHP;
        iMaxHP = GameManager.instance.iPlayerMaxHP;
        iScore = GameManager.instance.iScore;

        for (int i = 0; i < GameManager.instance.iNumCards; i++)
        {
            cards[i].SetPlayer(this);
        }

        tHP.text = "HP: " + iHP;
        tScore.text = "Score: " + iScore;
        tCard.text = "Current Card: " + iCurrentCard + "\n" + "EffectNumber: " + cards[iCurrentCard - 1].cardImmediate.iEffect;

        base.Start();
	}

    private void OnDisable()
    {
        GameManager.instance.iPlayerHP = iHP;
        GameManager.instance.iPlayerMaxHP = iMaxHP;
        GameManager.instance.iScore = iScore;
        GameManager.instance.SetCards(cards);
        GameManager.instance.iCurrentCard = iCurrentCard;
    }

    // Update is called once per frame
    void Update ()
    {
        if (iSkipMove > 0)
        {
            iSkipMove--;
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
        base.AttemptMove<T>(xDir, yDir);

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
        else if (other.tag == "Card")
        {
            for (int i = 0; i < GameManager.instance.iNumCards; i++)
            {
                if (cards[i].GenerateCard())
                {
                    tCard.text = "Current Card: " + iCurrentCard + "\n" + "EffectNumber: " + cards[iCurrentCard - 1].cardImmediate.iEffect;
                    other.gameObject.SetActive(false);
                    break;
                }
            }            
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        PushBack();

        iSkipMove = 25;
    }

    //Function for being pushed back when walking into an enemy
    public void PushBack()
    {
        Vector2 position = transform.position;
        Vector2 newDirection = new Vector2(-vDirection.x, -vDirection.y) * Time.deltaTime * pushBack;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(position, position + newDirection, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            transform.position += (new Vector3(newDirection.x, newDirection.y));
        }
    }

    //Function for being pushed back from the enemy code
    public void PushBack(Vector2 direction)
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
        }
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
            //DestroyObject(this);
        }
    }

    public void GainScore(int score)
    {
        iScore += score;
    }

    public void GainHP(int health)
    {
        iHP += health;
        if (iHP > iMaxHP)
            iHP = iMaxHP;
    }

    //////////
    //INPUTS//
    //////////

    //Movement of the player
    public void Movement(bool keyCodeW, bool keyCodeA, bool keyCodeS, bool keyCodeD)
    {
        int horizontal = 0;
        int vertical = 0;

        //Old movement method, also using arrows keys
        //horizontal = (int)Input.GetAxisRaw("Horizontal");
        //vertical = (int)Input.GetAxisRaw("Vertical");

        if (keyCodeW)
        {
            vertical = 1;
            if (keyCodeS)
            {
                vertical = 0;
            }
        }
        else if (keyCodeS)
        {
            vertical = -1;
        }
        if (keyCodeD)
        {
            horizontal = 1;
            if (keyCodeA)
            {
                horizontal = 0;
            }
        }
        else if (keyCodeA)
        {
            horizontal = -1;
        }

        //Stun from walking into wall
        if (iSkipMove > 0)
        {
            horizontal = 0;
            vertical = 0;
        }

        //Will make it only move in 4 way not 8 way
        //if (horizontal != 0)
        //{
        //    vertical = 0;
        //}

        AnimCheck(horizontal, vertical);

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    //Switch the currently selected card
    public void SwitchCard(KeyCode keyCode)
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            iCurrentCard--;
            if (iCurrentCard <= 0)
            {
                iCurrentCard = GameManager.instance.iNumCards;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            iCurrentCard++;
            if (iCurrentCard > GameManager.instance.iNumCards)
            {
                iCurrentCard = 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            iCurrentCard = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            iCurrentCard = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            iCurrentCard = 3;
        }

        tCard.text = "Current Card: " + iCurrentCard + "\n" + "EffectNumber: " + cards[iCurrentCard - 1].cardImmediate.iEffect;
    }

    public bool UseCard()
    {
        bool check = cards[iCurrentCard - 1].DestroyCard();
        tHP.text = "HP: " + iHP;
        tScore.text = "Score: " + iScore;
        tCard.text = "Current Card: " + iCurrentCard + "\n" + "EffectNumber: " + cards[iCurrentCard - 1].cardImmediate.iEffect;
        return check;
    }
}
