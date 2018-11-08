using UnityEngine;

public class ButtonScript : MonoBehaviour {


    public enum ButtonType
    {
        move, attack, item, end, none
    }

    public ButtonType myType;

    private void OnMouseEnter()
    {
        Cursor.visible = true;
        FindObjectOfType<GameManager>().ToggleFakeCursor();
    }

    private void OnMouseExit()
    {
        Cursor.visible = false;
        FindObjectOfType<GameManager>().ToggleFakeCursor();
    }

    private void OnMouseDown()
    {
        if (FindObjectOfType<GameManager>().activeUnit)
        {
            if (myType == ButtonType.move)
            {
                FindObjectOfType<GameManager>().ShowTilesInMoveReach();
            }
            if (myType == ButtonType.attack)
            {
                FindObjectOfType<GameManager>().ShowTilesInAttackReach();
            }
            if (myType == ButtonType.item)
            {
                FindObjectOfType<GameManager>().ShowTilesInMoveReach();
            }
        }
        if (myType == ButtonType.end)
        {
            FindObjectOfType<GameManager>().ShowTilesInMoveReach();
        }

    }

}
