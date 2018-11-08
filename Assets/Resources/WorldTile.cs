
using System.Collections.Generic;
using UnityEngine;

public class WorldTile: MonoBehaviour {

    public bool hasUnit, inMoveReach, visited, unpassable, inAttackReach;

    public WorldTile parent;

    public short tileDistance;

    GameManager gm;

    [SerializeField] GameObject moveReachEffect, attackReachEffect;

    GameObject moveEffect, attackEffect;

    public List<WorldTile> adjacentTiles;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        moveEffect = Instantiate(moveReachEffect, transform.position, Quaternion.identity);
        attackEffect = Instantiate(attackReachEffect, transform.position, Quaternion.identity);
        moveEffect.SetActive(false);
        attackEffect.SetActive(false);
    }

    private void Update()
    {
        if (inMoveReach)
        {
            if (Input.GetMouseButtonDown(0) && (Vector2)gm.cursorObject.transform.position == (Vector2)transform.position)
            {
                gm.MakePath(this);
                gm.ResetTiles();
            }
            if (!moveEffect.activeInHierarchy)
            {
                moveEffect.SetActive(true);
            }
        }
        else
        {
            if (moveEffect.activeInHierarchy)
            {
                moveEffect.SetActive(false);
            }
        }
        if (inAttackReach)
        {
            if (Input.GetMouseButtonDown(0) && (Vector2)gm.cursorObject.transform.position == (Vector2)transform.position)
            {
                gm.activeUnit.PerformAttack(this);
            }
            if (!attackEffect.activeInHierarchy)
            {
                attackEffect.SetActive(true);
            }
        }
        else
        {
            if (attackEffect.activeInHierarchy)
            {
                attackEffect.SetActive(false);
            }
        }
        
    }

    public void FindNeighbours(bool includeUnits)
    {
        adjacentTiles.Clear();
        CheckTile(Vector2.up, includeUnits);
        CheckTile(-Vector2.up, includeUnits);
        CheckTile(Vector2.right, includeUnits);
        CheckTile(-Vector2.right, includeUnits);
    }

    void CheckTile(Vector2 dir, bool includeUnits)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll((Vector2)transform.position + dir, new Vector2(0.1f, 0.1f), 0);

        foreach(Collider2D col in colliders)
        {
            WorldTile t = col.GetComponent<WorldTile>();
            if(t != null && !t.unpassable)
            {
                if (includeUnits)
                {
                    adjacentTiles.Add(col.GetComponent<WorldTile>());
                }
                else
                {
                    if (!t.hasUnit)
                    {
                        adjacentTiles.Add(col.GetComponent<WorldTile>());
                    }
                }
            }
        }
    }
}
