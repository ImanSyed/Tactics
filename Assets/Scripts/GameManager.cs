using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public List<UnitScript> units;
    public WorldTile[] tiles;

    public UnitScript activeUnit;

    List<WorldTile> tilesInReach;

    public Stack<WorldTile> path = new Stack<WorldTile>();

    float snapValue = 1f;

    public GameObject cursorObject, activeEffect;

    [SerializeField] GameObject UIObject;

    short playerTurn = 1;

    public bool friendlyFire;
    
	void Start () {

        tiles = FindObjectsOfType<WorldTile>();

        units = FindObjectsOfType<UnitScript>().ToList();

        UpdateMap();


        Cursor.visible = false;
	}

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        cursorObject.transform.position = new Vector3(Round(mousePos.x), Round(mousePos.y), -9);

        if (activeUnit)
        {
            activeEffect.transform.position = activeUnit.transform.position;
            if (!UIObject.activeInHierarchy)
            {
                UIObject.SetActive(true);
            }
        }
        else if (UIObject.activeInHierarchy)
        {
            UIObject.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            foreach (UnitScript u in units)
            {
                if ((Vector2)u.transform.position == (Vector2)cursorObject.transform.position && u.playerNum == playerTurn)
                {
                    activeUnit = u;
                    activeUnit.ActivateUnit();
                }
            }
        }
    }

    public void ToggleFakeCursor()
    {
        if (cursorObject.GetComponent<Renderer>().enabled)
        {
            cursorObject.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            cursorObject.GetComponent<Renderer>().enabled = true;
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

    public void ShowTilesInAttackReach()
    {
        ResetTiles();
        WorldTile activeTile = null;

        activeUnit.attacking = true;

        foreach (WorldTile tile in tiles)
        {
            if (tile.transform.position == activeUnit.transform.position)
            {
                activeTile = tile;
            }
            tile.FindNeighbours(true);
        }

        Queue<WorldTile> process = new Queue<WorldTile>();

        process.Enqueue(activeTile);
        activeTile.visited = true;

        while (process.Count > 0)
        {
            WorldTile t = process.Dequeue();

            if (t.transform.position != activeUnit.transform.position)
            {
                t.inAttackReach = true;
            }

            if (t.tileDistance < activeUnit.range)
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
        activeTile.inMoveReach = false;
    }

    public void ShowTilesInMoveReach()
    {
        ResetTiles();
        WorldTile activeTile = null;

        foreach(WorldTile tile in tiles)
        {
            if(tile.transform.position == activeUnit.transform.position)
            {
                activeTile = tile;
            }
            tile.FindNeighbours(false);
        }

        Queue<WorldTile> process = new Queue<WorldTile>();

        process.Enqueue(activeTile);
        activeTile.visited = true;

        while(process.Count > 0)
        {
            WorldTile t = process.Dequeue();

            if (!t.hasUnit)
            {
                t.inMoveReach = true;
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
        activeTile.inMoveReach = false;
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

    public void ResetTiles()
    {
        foreach(WorldTile tile in tiles)
        {
            tile.inMoveReach = false;
            tile.visited = false;
            tile.parent = null;
            tile.hasUnit = false;
            tile.inAttackReach = false;
            tile.tileDistance = 0;
        }
        UpdateMap();
    }

    public void EndTurn()
    {
        ResetTiles();
        activeUnit = null;
        activeEffect.transform.position = new Vector2(-1000, -1000);
        if(playerTurn == 1)
        {
            playerTurn = 2;
        }
        else
        {
            playerTurn = 1;
        }
        foreach(ButtonScript button in FindObjectsOfType<ButtonScript>())
        {
            button.activated = false;
        }
    }

    public void DeactivateUnits()
    {
        foreach(UnitScript u in units)
        {
            u.active = false;
        }
    }
}
