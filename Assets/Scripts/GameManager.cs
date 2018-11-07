using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public UnitScript[] units;
    public WorldTile[] tiles;

    public UnitScript activeUnit;

    List<WorldTile> tilesInReach;

    public Stack<WorldTile> path = new Stack<WorldTile>();

    float snapValue = 1f;

    public GameObject cursorObject;

	void Start () {

        tiles = FindObjectsOfType<WorldTile>();

        units = FindObjectsOfType<UnitScript>();

        UpdateMap();

        Cursor.visible = false;
	}

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        cursorObject.transform.position = new Vector3(Round(mousePos.x), Round(mousePos.y), -1);
        
        if (Input.GetMouseButtonDown(0))
        {
            foreach (UnitScript u in units)
            {
                if ((Vector2)u.transform.position == (Vector2)cursorObject.transform.position)
                {
                    u.ActivateUnit();
                    activeUnit = u;
                    ShowTilesInReach();
                }
            }
        }
    }

    private float Round(float input)
    {
        return (snapValue * Mathf.Round(input / snapValue) + 0.5f);
    }

    public void UpdateMap()
    {
        
        foreach(WorldTile t in tiles)
        {
            foreach(UnitScript u in units)
            {
                if(!t.hasUnit && t.transform.position == u.transform.position)
                {
                    t.hasUnit = true;
                }
            }
        }
    }

    public void ShowTilesInReach()
    {
        WorldTile activeTile = null;

        foreach(WorldTile tile in tiles)
        {
            if(tile.transform.position == activeUnit.transform.position)
            {
                activeTile = tile;
            }
            tile.FindNeighbours();
        }

        Queue<WorldTile> process = new Queue<WorldTile>();

        process.Enqueue(activeTile);
        activeTile.visited = true;

        while(process.Count > 0)
        {
            WorldTile t = process.Dequeue();

            if (!t.hasUnit)
            {
                t.inReach = true;
            }
            if (t.tileDistance < activeUnit.movesRemaining)
            {
                foreach (WorldTile tile in t.adjacentTiles)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.tileDistance = (short)(1 + t.tileDistance);
                        process.Enqueue(tile);
                    }
                }
            }
        }
        activeTile.inReach = false;
    }

    public void MakePath(WorldTile tile)
    {
        path.Clear();
        
        WorldTile next = tile;
        while (next)
        {
            path.Push(next);
            next = next.parent;
        }
        activeUnit.endTile = tile;
        activeUnit.moving = true;
    }

    public void HideTilesInReach()
    {
        foreach(WorldTile tile in tiles)
        {
            tile.inReach = false;
            tile.visited = false;
            tile.parent = null;
            tile.hasUnit = false;
            tile.tileDistance = 0;
        }
        UpdateMap();
    }

    public void DeactivateUnits()
    {
        foreach(UnitScript u in units)
        {
            u.active = false;
        }
    }
}
