
using System.Collections.Generic;
using UnityEngine;

public class WorldTile: MonoBehaviour {

    public string TileType { get; set; }

    public bool hasUnit, inReach, visited;

    public Vector3Int TilePosition { get; set; }

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

    private void OnMouseDown()
    {
        if (gm.activeUnit && inReach)
        {
            gm.activeUnit.Move(transform.position);
            gm.HideTilesInReach();
        }
    }

    public void FindNeighbours()
    {
        CheckTile(Vector2.up);
        CheckTile(-Vector2.up);
        CheckTile(Vector2.right);
        CheckTile(-Vector2.right);
    }

    void CheckTile(Vector2 dir)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll((Vector2)transform.position + dir, new Vector2(0.25f, 0.25f), 0);

        foreach(Collider2D col in colliders)
        {
            if(col.GetComponent<WorldTile>() != null && col.GetComponent<WorldTile>().TileType != "Unpassable" && !col.GetComponent<WorldTile>().hasUnit)
            {
               adjacentTiles.Add(col.GetComponent<WorldTile>());
            }
        }
    }
}
