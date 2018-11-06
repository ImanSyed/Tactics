using UnityEngine;

public class UnitScript : MonoBehaviour {

    GameManager gm;
    public short moves = 3, range = 5, health = 3, movesRemaining;
    [SerializeField] float moveSpeed = 0.005f;
    public bool active;
    bool move;
    Vector2 destination, startPos;

    void Start () {
        gm = FindObjectOfType<GameManager>();
	}
	
	void Update () {
        if (move)
        {
            if((Vector2)transform.position == destination)
            {
                move = false;
                gm.UpdateMap();
                active = false;
            }
            else
            {
                if(transform.position.x == destination.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, destination.y), moveSpeed);
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(destination.x, transform.position.y), moveSpeed);
                }
            } 
        }
	}

    public void Move(Vector2 location)
    {
        startPos = transform.position;
        destination = location;
        move = true;
        movesRemaining -= (short)(Vector2.Distance(new Vector2(destination.x, transform.position.y), transform.position) + Vector2.Distance(new Vector2(transform.position.x, destination.y), transform.position));

    }

    void OnMouseDown()
    {
        if (!active)
        {
            movesRemaining = moves;
            gm.DeactivateUnits();
            gm.HideTilesInReach();
            active = true;
            gm.IAmActive(this);
        }
    }
}
