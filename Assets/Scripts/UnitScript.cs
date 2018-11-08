using UnityEngine;

public class UnitScript : MonoBehaviour {

    GameManager gm;
    public short moves = 3, range = 5, health = 3, movesRemaining;
    [SerializeField] float moveSpeed = 0.005f;
    public bool active, moving, canAttack;
    Vector2 destination, startPos;
    public WorldTile endTile;
    short counter;

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

    public void InitiateAttack()
    {
        canAttack = true;
        gm.ShowTilesInAttackReach();
    }

    public void ActivateUnit()
    {
        movesRemaining = moves;
        gm.DeactivateUnits();
        gm.HideTilesInReach();
        active = true;
    }
}
