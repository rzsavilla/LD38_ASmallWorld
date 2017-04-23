using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movable : MonoBehaviour {

    public int iSpeed = 5;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;

    private Vector2 prevPos;
    private RaycastHit2D hit;

    // Use this for initialization
    protected virtual void Start () {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }
	
	protected bool Move(int xDir, int yDir)
    {
        Vector2 position = transform.position;
        Vector2 direction = new Vector2(xDir, yDir) * Time.deltaTime;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(position, position + direction, blockingLayer);
        boxCollider.enabled = true;
        if (hit.transform == null)
        {
            rb2D.MovePosition(position + direction);
            transform.position = position + direction;
            position = transform.position;
            for (int i = 1; i < iSpeed; i++)
            {
                boxCollider.enabled = false;
                hit = Physics2D.Linecast(position, position + direction, blockingLayer);
                boxCollider.enabled = true;

                if (hit.transform == null)
                {
                    rb2D.MovePosition(position + direction);
                    transform.position = position + direction;
                    position = transform.position;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool Move(int xDir, int yDir, float speed)
    {
        Vector3 end = transform.position + (new Vector3(xDir, yDir) * Time.deltaTime * speed);

        rb2D.MovePosition(end);
        transform.position = end;

        for (int i = 1; i < iSpeed; i++)
        {
            end = transform.position + (new Vector3(xDir, yDir) * Time.deltaTime * speed);

            rb2D.MovePosition(end);
            transform.position = end;
        }

        return true;

        //Original movement code. Using this will cause collisions with objects while travelling
        /*Vector2 position = transform.position;
        Vector2 direction = new Vector2(xDir, yDir) * Time.deltaTime * speed;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(position, position + direction, blockingLayer);
        boxCollider.enabled = true;
        if (hit.transform == null)
        {
            rb2D.MovePosition(position + direction);
            transform.position = position + direction;
            position = transform.position;
            for (int i = 1; i < iSpeed; i++)
            {
                boxCollider.enabled = false;
                hit = Physics2D.Linecast(position, position + direction, blockingLayer);
                boxCollider.enabled = true;

                if (hit.transform == null)
                {
                    rb2D.MovePosition(position + direction);
                    transform.position = position + direction;
                    position = transform.position;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }*/
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T:Component
    {
        bool canMove = Move(xDir, yDir);

        if (canMove)
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir, float speed)
        where T : Component
    {
        bool canMove = Move(xDir, yDir, speed);

        if (canMove)
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
