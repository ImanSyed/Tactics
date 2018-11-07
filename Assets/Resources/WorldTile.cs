
using System.Collections.Generic;
using UnityEngine;

public class WorldTile: MonoBehaviour {

    public bool hasUnit, inReach, visited, unpassable;

    public WorldTile parent;

    public short tileDistance;

    GameManager gm;

    [SerializeField] GameObject reachEffect;

    GameObject effect;

    public List<WorldTile> adjacentTiles;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        effect = Instantiate(reachEffect, transform.position, Quaternion.identity);
        effect.SetActive(false);
    }

    private void Update()
    {
        if (inReach)
        {
            if (Input.GetMouseButtonDown(0) && (Vector2)gm.cursorObject.transform.position == (Vector2)transform.position)
            {
                gm.MakePath(this);
                gm.HideTilesInReach();
            }
            if (!effect.activeInHierarchy)
            {
                effect.SetActive(true);
            }
        }
        else
        {
            if (effect.activeInHierarchy)
            {
                effect.SetActive(false);
            }
        }

    }

    public void FindNeighbours()
    {
        adjacentTiles.Clear();
        CheckTile(Vector2.up);
        CheckTile(-Vector2.up);
        CheckTile(Vector2.right);
        CheckTile(-Vector2.right);
    }

    void CheckTile(Vector2 dir)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll((Vector2)transform.position + dir, new Vector2(0.1f, 0.1f), 0);

        foreach(Collider2D col in colliders)
        {
            WorldTile t = col.GetComponent<WorldTile>();
            if(t != null && !t.unpassable && !t.hasUnit)
            {
                adjacentTiles.Add(col.GetComponent<WorldTile>());
            }
        }
    }
}
