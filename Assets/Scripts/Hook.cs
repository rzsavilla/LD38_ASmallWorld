using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {

    public bool bUse = true;
    public bool bTarget = true;
    Player player;
    public Vector2 vDirection;
    public int iSpeed = 10;
    public int iPushBack = 20;
    //Start distance from player. Will need to change if change hook speed
    public float fStartDistance = 0.7f;

    private int iCurrentDistance = 0;
    private int iMaxDistance = 100;
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
        bTarget = true;

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
        if (bTarget)
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
                        bTarget = false;
                        if (hit.transform.gameObject.tag == "Enemy")
                        {
                            hit.transform.gameObject.GetComponent<Enemy>().PushBack(vDirection);
                        }
                        break;
                    }
                }
            }
            else
            {
                bTarget = false;
                if (hit.transform.gameObject.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<Enemy>().PushBack(vDirection);
                }
            }
        }
        else
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
    }

    public void Return()
    {
        if (bTarget)
        {
            bTarget = false;
        }
    }
}
