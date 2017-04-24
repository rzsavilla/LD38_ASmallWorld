using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Movable {

    public int iState = 0;
    public float restartLevelDelay = 1f;
    public float pushBack = 10f;
    public Text tHP;
    public Text tScore;
    public Text tCard;
    public int iHookDamage = 2;
    public int iCurrentCard;
    public int iSkipMove = 0;
    public int iSkipAmount = 20;
    public int iAttackCooldown = 0;
    public int iMaxAttackCooldown = 20;
    public int iHookLength = 200;
    public float fHookMove = 2f;
    public GameObject hook;
    public List<GameObject> hooks;
    private LineRenderer chainRender;

    private Animator animator;
    private int iHP;
    private int iMaxHP;
    private int iScore;
    private Vector2 vDirection;
    private List<Card> cards;
    public bool bHookUse = false;

    //Score values for pickups
    private int iPickup1 = 5;
    private int iPickup2 = 10;
    private int iPickup3 = 20;

    // Use this for initialization
    protected override void Start ()
    {
        vDirection = new Vector2(0f, -1f);

        animator = GetComponent<Animator>();
        cards = GameManager.instance.GetCards();
        iCurrentCard = GameManager.instance.iCurrentCard;
        iHP = GameManager.instance.iPlayerHP;
        iMaxHP = GameManager.instance.iPlayerMaxHP;
        iScore = GameManager.instance.iScore;
        iHookDamage = GameManager.instance.iHookDamage;

        for (int i = 0; i < GameManager.instance.iNumCards; i++)
        {
            cards[i].SetPlayer(this);
        }

        tHP.text = "HP: " + iHP;
        tScore.text = "Score: " + iScore;
        tCard.text = "Current Card: " + iCurrentCard + "\n" + "EffectNumber: " + cards[iCurrentCard - 1].cardImmediate.iEffect;

        base.Start();

        chainRender = GetComponent<LineRenderer>();
        //chainRender.startColor = Color.black;
        //chainRender.endColor = Color.black;
    }

    private void OnDisable()
    {
        GameManager.instance.iPlayerHP = iHP;
        GameManager.instance.iPlayerMaxHP = iMaxHP;
        GameManager.instance.iScore = iScore;
        GameManager.instance.SetCards(cards);
        GameManager.instance.iCurrentCard = iCurrentCard;
        GameManager.instance.iHookDamage = iHookDamage;
    }

    // Update is called once per frame
    void Update ()
    {
        if (iSkipMove > 0)
        {
            iSkipMove--;
        }
        if (!bHookUse)
        {
            Vector3[] linePositions = new Vector3[2];
            linePositions[0] = transform.position;         //Start position
            linePositions[1] = transform.position;         //End
            linePositions[0].z = 1;
            linePositions[1].z = 1;
            chainRender.SetPositions(linePositions);
            if (iAttackCooldown > 0)
            {
                iAttackCooldown--;
            }
        }
        else
        {
            if (chainRender != null)
            {
                Vector3[] linePositions = new Vector3[2];
                linePositions[0] = transform.position;         //Start position
                linePositions[1] = hooks[hooks.Count - 1].GetComponent<Hook>().transform.position;     //End Position
                chainRender.SetPositions(linePositions);
                //Chain in front of player
                if (chainRender != null) chainRender.sortingLayerName = "Units";
                if (chainRender != null) chainRender.sortingOrder = 999;
            }
        }
        if (hooks.Count > 0)
        {
            if (hooks[hooks.Count - 1].GetComponent<Hook>().iTarget == 2)
            {
                iState = 1;
            }
            else
            {
                iState = 0;
            }
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

        if (bHookUse)
        {
            chainRender.SetPosition(0, transform.position);
        }
        else
        {
            Vector3 position = transform.position;
            position.z = 1;
            chainRender.SetPosition(0, position);
            chainRender.SetPosition(1, position);
        }
        //CheckIfGameOver();
    }

    //Attempt Move, but with a speed multiplier
    protected override void AttemptMove<T>(float xDir, float yDir, float speed)
    {
        vDirection = new Vector2(xDir, yDir);
        base.AttemptMove<T>(xDir, yDir, speed);

        if (bHookUse)
        {
            chainRender.SetPosition(0, transform.position);
        }
        else
        {
            Vector3 position = transform.position;
            position.z = 1;
            chainRender.SetPosition(0, position);
            chainRender.SetPosition(1, position);
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

        iSkipMove = iSkipAmount;

        ReturnHook();
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

        //If travelling by hook, ignore movement
        if (hooks.Count > 0)
        {
            if (hooks[hooks.Count - 1].GetComponent<Hook>().iTarget == 2)
            {
                horizontal = 0;
                vertical = 0;
            }
            else if (horizontal != 0 || vertical != 0)
            {
                AttemptMove<Wall>(horizontal, vertical);
            }
        }
        else if (horizontal != 0 || vertical != 0)
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

    public void Attack()
    {
        if (iAttackCooldown <= 0 && !bHookUse)
        {
            if (hooks.Count > 0)
            {
                for(int i = 0; i < hooks.Count; i++)
                {
                    DestroyImmediate(hooks[0], true);
                }
            }
            animator.SetBool("playerAttack", true);
            hooks.Add(Instantiate(hook, transform.position, Quaternion.identity));
            hooks[hooks.Count-1].SetActive(true);
            if (hooks[hooks.Count-1].GetComponent<Hook>().Attack(this, vDirection, iHookLength))
            {
                iAttackCooldown = iMaxAttackCooldown;
                bHookUse = true;
            }   
            else
            {
                DestroyHook();
            }
        }
        else
        {
            //Can't attack until cooldown over
        }
    }

    public void ReturnHook()
    {
        if (bHookUse)
        {
            hooks[hooks.Count - 1].GetComponent<Hook>().Return();
        }
    }

    public void DestroyHook()
    {
        for (int i = 0; i < hooks.Count; i++)
        {
            DestroyImmediate(hooks[0], true);
        }
        hooks.Clear();

        bHookUse = false;

        animator.SetBool("playerAttack", false);
    }

    public void EnemyHit(GameObject enemy)
    {
        enemy.GetComponent<Enemy>().Hit(iHookDamage);
        if (hooks[hooks.Count-1].GetComponent<Hook>().iTarget == 2)
        {
            enemy.GetComponent<Enemy>().PushBack(vDirection);
        }
    }

    public void TravelTo(Vector3 destination)
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (destination.x - transform.position.x > 0)
        {
            horizontal = 1;
            if (destination.x - transform.position.x < 1f)
            {
                horizontal = destination.x - transform.position.x;
            }
        }
        
        else if (destination.x - transform.position.x < 0)
        {
            horizontal = -1;
            if (destination.x - transform.position.x > -1f)
            {
                horizontal = destination.x - transform.position.x;
            }
        }
        if (destination.y - transform.position.y > 0)
        {
            vertical = 1;
            if (destination.y - transform.position.y < 1f)
            {
                vertical = destination.y - transform.position.y;
            }
        }
        else if (destination.y - transform.position.y < 0)
        {
            vertical = -1;
            if (destination.y - transform.position.y > -1f)
            {
                vertical = destination.y - transform.position.y;
            }
        }

        AttemptMove<Wall>(horizontal, vertical, fHookMove);
    }
}
