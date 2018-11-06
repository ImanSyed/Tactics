using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public UnitScript[] units;
    public WorldTile[] tiles;

    public UnitScript activeUnit;

    List<WorldTile> tilesInReach;

	void Start () {

        tiles = FindObjectsOfType<WorldTile>();

        units = FindObjectsOfType<UnitScript>();

        foreach(WorldTile t in tiles)
        {
            t.TilePosition = new Vector3Int ((int)Mathf.Ceil(t.gameObject.transform.position.x), (int)Mathf.Ceil(t.gameObject.transform.position.y), 0);
        }
        UpdateMap();
	}

    public void UpdateMap()
    {
        foreach(WorldTile t in tiles)
        {
            foreach(UnitScript u in units)
            {
                if(t.transform.position == u.transform.position)
                {
                    t.hasUnit = true;
                }
            }
        }
    }

    public void IAmActive(UnitScript unit)
    {
        activeUnit = unit;
        ShowTilesInReach();
    }

    public void ShowTilesInReach()
    {
        WorldTile activeTile = new WorldTile();

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

            t.inReach = true;

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
