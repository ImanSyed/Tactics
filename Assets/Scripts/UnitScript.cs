using UnityEngine;
using System;
using System.Text.RegularExpressions;



public class UnitScript : MonoBehaviour {

    GameManager gm;
    [HideInInspector]
    public short moves = 3, attackRange = 5, health = 3, damage = 2, spellRange = 4, mana = 3, manaCost = 1, movesRemaining;
    [SerializeField] float moveSpeed;
    [HideInInspector]
    public bool active, moving, attacking, casting;
    Vector2 destination, startPos;
    [HideInInspector]
    public WorldTile endTile;
    short counter, distance;

    [SerializeField] TextAsset data;
    [SerializeField] string className;

    public short playerNum;

    string spell;
    string[] myValues;
    Animator animator;

    void Start () {
        gm = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        data = Resources.Load("Values") as TextAsset;
        string[] lines = Regex.Split(data.text, @"\r\n|\n\r|\r|\n");
        switch (className)
        {
            case "Priest":
                myValues = Regex.Split(lines[1], ",");
                break;
            case "Mage":
                myValues = Regex.Split(lines[2], ",");
                break;
            case "Warrior":
                myValues = Regex.Split(lines[3], ",");
                break;
        }

            health = (short)int.Parse(myValues[1]);
            mana = (short)int.Parse(myValues[2]);
            moves = (short)int.Parse(myValues[3]);
            damage = (short)int.Parse(myValues[4]);
            spell = myValues[5];
            manaCost = (short)int.Parse(myValues[6]);
            spellRange = (short)int.Parse(myValues[7]);
            attackRange = (short)int.Parse(myValues[8]);
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
                    movesRemaining--;
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
                if (dir.normalized == Vector2.right)
                {
                    if (animator.GetInteger("MoveDirection") != 3)
                    {
                        animator.SetInteger("MoveDirection", 3);
                        GetComponent<SpriteRenderer>().flipX = true;
                    }
                }
                if (dir.normalized == Vector2.up)
                {
                    if (animator.GetInteger("MoveDirection") != 1)
                    {
                        animator.SetInteger("MoveDirection", 1);
                        GetComponent<SpriteRenderer>().flipX = false;
                    }
                }
                if (dir.normalized == -Vector2.up)
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

    public void CastSpell(WorldTile t)
    {
        if (t.hasUnit)
        {
            foreach (UnitScript u in FindObjectsOfType<UnitScript>())
            {
                if ((Vector2)u.transform.position == (Vector2)t.transform.position)
                {
                    if (spell == "Fireball")
                    {
                        u.DealDamage(5);
                        mana--;
                    }
                    if(spell == "Heal")
                    {
                        u.DealDamage(-3);
                        mana--;
                    }
                    casting = false;
                    return;
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