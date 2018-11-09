using System.Collections.Generic;
using System.Collections;
using UnityEngine;



public class UnitScript : MonoBehaviour {

    GameManager gm;
    public short moves = 3, range = 5, health = 3, damage = 2, movesRemaining;
    [SerializeField] float moveSpeed = 0.005f;
    public bool active, moving, attacking;
    Vector2 destination, startPos;
    public WorldTile endTile;
    short counter;

    public short playerNum;

    void Start () {
        gm = FindObjectOfType<GameManager>();
	}

    private void Update()
    {
        if (moving)
        {
            if(transform.position == endTile.transform.position)
            {
                moving = false;
                gm.ResetTiles();
            }
            else
            {
                if(transform.position == gm.path.Peek().transform.position)
                {
                    gm.path.Pop();
                }
                transform.position = Vector2.MoveTowards(transform.position, gm.path.Peek().transform.position, moveSpeed);
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
