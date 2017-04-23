using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {

    public bool bUse = true;
    //The current state of the Hook. 0 = returning, 1 = travelling, 2 = hooked to wall
    public int iTarget = 1;
    Player player;
    public Vector2 vDirection;
    public int iSpeed = 10;
    public int iPushBack = 20;
    //Start distance from player. Will need to change if change hook speed
    public float fStartDistance = 0.7f;

    private int iCurrentDistance = 0;
    private int iMaxDistance = 200;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private RaycastHit2D hit;
    public LayerMask blockingLayer;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    public bool Attack(Player player, Vector2 direction, int hookLength)
    {
        iCurrentDistance = 0;
        iMaxDistance = hookLength;
        this.player = player;
        vDirection = direction;
        transform.position += new Vector3(direction.x, direction.y) * fStartDistance;
        iTarget = 1;

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(transform.position, new Vector3(transform.position.x + float.Epsilon, transform.position.y + float.Epsilon), blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (iTarget == 1)
        {
            Vector3 end = transform.position + (new Vector3(vDirection.x, vDirection.y) * Time.deltaTime);

            boxCollider.enabled = false;
            hit = Physics2D.Linecast(transform.position, end, blockingLayer);
            boxCollider.enabled = true;
            if (hit.transform == null)
            {
                rb2D.MovePosition(end);
                transform.position = end;
                iCurrentDistance++;
                if (iCurrentDistance >= iMaxDistance)
                {
                    Return();
                }
                end += new Vector3(vDirection.x, vDirection.y) * Time.deltaTime;
                for (int i = 1; i < iSpeed; i++)
                {
                    boxCollider.enabled = false;
                    hit = Physics2D.Linecast(transform.position, end, blockingLayer);
                    boxCollider.enabled = true;

                    if (hit.transform == null)
                    {
                        rb2D.MovePosition(end);
                        transform.position = end;
                        iCurrentDistance++;
                        if (iCurrentDistance >= iMaxDistance)
                        {
                            Return();
                        }
                        end += new Vector3(vDirection.x, vDirection.y) * Time.deltaTime;
                    }
                    else
                    {
                        iTarget = 0;
                        //Hit an enemy, so push them back
                        if (hit.transform.gameObject.tag == "Enemy")
                        {
                            hit.transform.gameObject.GetComponent<Enemy>().PushBack(vDirection);
                        }
                        //Hit a wall, need to travel to it
                        else if (hit.transform.gameObject.tag == "Wall")
                        {
                            iTarget = 2;
                        }
                        break;
                    }
                }
            }
            else
            {
                iTarget = 0;
                if (hit.transform.gameObject.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<Enemy>().PushBack(vDirection);
                }
            }
        }
        else if (iTarget == 0)
        {
            vDirection = player.transform.position - transform.position;
            vDirection.Normalize();
            Vector3 end = transform.position + (new Vector3(vDirection.x, vDirection.y) * Time.deltaTime);

            rb2D.MovePosition(end);
            transform.position = end;

            if (((player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x))
                + ((player.transform.position.y - transform.position.y) * (player.transform.position.y - transform.position.y)) > 0.5f)
            {
                for (int i = 1; i < iSpeed; i++)
                {
                    end += new Vector3(vDirection.x, vDirection.y) * Time.deltaTime;

                    rb2D.MovePosition(end);
                    transform.position = end;

                    if (((player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x))
                        + ((player.transform.position.y - transform.position.y) * (player.transform.position.y - transform.position.y)) > 0.5f)
                    {

                    }
                    else
                    {
                        player.DestroyHook();
                        break;
                    }
                }
            }
            else
            {
                player.DestroyHook();
            }
        }
        else if (iTarget == 2)
        {
            //Move player to us
            player.TravelTo(transform.position);

            if (((player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x))
                + ((player.transform.position.y - transform.position.y) * (player.transform.position.y - transform.position.y)) > 0.5f)
            {

            }
            else
            {
                iTarget = 0;
                player.iSkipMove = player.iSkipAmount;
            }
        }
    }

    public void Return()
    {
        if (iTarget == 1)
        {
            iTarget = 0;
        }
    }
}
