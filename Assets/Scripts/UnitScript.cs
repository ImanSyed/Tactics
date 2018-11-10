using System.Collections.Generic;
using System.Collections;
using UnityEngine;



public class UnitScript : MonoBehaviour {

    GameManager gm;
    public short moves = 3, range = 5, health = 3, damage = 2, movesRemaining;
    [SerializeField] float moveSpeed;
    public bool active, moving, attacking;
    Vector2 destination, startPos;
    public WorldTile endTile;
    short counter;

    public short playerNum;

    Animator animator;

    void Start () {
        gm = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
	}

    private void Update()
    {
        if (moving)
        {
            if(transform.position == endTile.transform.position)
            {
                moving = false;
                if (animator.GetInteger("MoveDirection") != 0)
                {
                    animator.SetInteger("MoveDirection", 0);
                }
                gm.ResetTiles();
            }
            else
            {
                if(transform.position == gm.path.Peek().transform.position)
                {
                    gm.path.Pop();
                }
                transform.position = Vector2.MoveTowards(transform.position, gm.path.Peek().transform.position, moveSpeed);
                Vector2 dir = (Vector2)transform.position - (Vector2)gm.path.Peek().transform.position;
                if (dir.normalized == -Vector2.right)
                {
                    if (animator.GetInteger("MoveDirection") != 3)
                    {
                        animator.SetInteger("MoveDirection", 3);
                        GetComponent<SpriteRenderer>().flipX = false;
                    }
                }
                else if (dir.normalized == Vector2.right)
                {
                    if (animator.GetInteger("MoveDirection") != 3)
                    {
                        animator.SetInteger("MoveDirection", 3);
                        GetComponent<SpriteRenderer>().flipX = true;
                    }
                }
                else if (dir.normalized == Vector2.up)
                {
                    if (animator.GetInteger("MoveDirection") != 1)
                    {
                        animator.SetInteger("MoveDirection", 1);
                        GetComponent<SpriteRenderer>().flipX = false;
                    }
                }
                else if (dir.normalized == -Vector2.up)
                {
                    if (animator.GetInteger("MoveDirection") != 2)
                    {
                        animator.SetInteger("MoveDirection", 2);
                        GetComponent<SpriteRenderer>().flipX = false;
                    }
                }
            }
        }
    }

    public void PerformAttack(WorldTile t)
    {
        if (t.hasUnit)
        {
            foreach(UnitScript u in FindObjectsOfType<UnitScript>())
            {
                if((Vector2)u.transform.position == (Vector2)t.transform.position)
                {
                    if (gm.friendlyFire || u.playerNum != playerNum)
                    {
                        u.DealDamage(damage);
                        attacking = false;
                        return;
                    }
                }
            }
        }
    }

    public void DealDamage(short dmg)
    {
        health -= dmg;
        if(health <= 0)
        {
            gm.units.Remove(this);
            DestroyUnit();
        }
        gm.ResetTiles();
    }

    public void ActivateUnit()
    {
        movesRemaining = moves;
        gm.DeactivateUnits();
        gm.ResetTiles();
        active = true;
    }

    void DestroyUnit()
    {
        Destroy(gameObject);
    }
}