using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    [HideInInspector] public List<UnitScript> units;
    [HideInInspector] public WorldTile[] tiles;
    [HideInInspector] public UnitScript activeUnit;
    [HideInInspector] public Stack<WorldTile> path = new Stack<WorldTile>();

    List<WorldTile> tilesInReach;

    float snapValue = 1f;

    public GameObject cursorObject, activeEffect;

    [SerializeField] GameObject UIObject;

    short playerTurn = 1, p1Potions = 2, p2Potions = 2;

    public bool friendlyFire, unitLocked;

    [SerializeField] Text hp, sp, mp;

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
            hp.text = activeUnit.health.ToString();
            mp.text = activeUnit.movesRemaining.ToString();
            sp.text = activeUnit.mana.ToString();
            if (!UIObject.activeInHierarchy)
            {
                UIObject.SetActive(true);
            }
        }
        else if (UIObject.activeInHierarchy)
        {
            UIObject.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0) && !unitLocked)
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

            if (t.tileDistance < activeUnit.attackRange)
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
        activeTile.inAttackReach = false;
    }

    public void ShowTilesInSpellReach()
    {
        ResetTiles();
        WorldTile activeTile = null;

        activeUnit.casting = true;

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

             t.inSpellReach = true;

            if (t.tileDistance < activeUnit.attackRange)
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
            tile.inSpellReach = false;
            tile.tileDistance = 0;
        }
        UpdateMap();
    }

    public void EndTurn()
    {
        unitLocked = false;
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

    public void GivePotion()
    {
        switch (playerTurn)
        {
            case 1:
                if (p1Potions > 0)
                {
                    activeUnit.health += 3;
                    activeUnit.mana += 3;
                    p1Potions--;
                }
                break;
            case 2:
                if (p2Potions > 0)
                {
                    activeUnit.health += 3;
                    activeUnit.mana += 3;
                    p2Potions--;
                }
                break;
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
