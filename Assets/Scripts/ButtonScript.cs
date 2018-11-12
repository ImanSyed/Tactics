using UnityEngine;

public class ButtonScript : MonoBehaviour {


    public enum ButtonType
    {
        move, attack, item, ability, end, none
    }

    public ButtonType myType;

    GameManager gm;

    public bool activated;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnMouseEnter()
    {
        Cursor.visible = true;
        gm.ToggleFakeCursor();
    }

    private void OnMouseExit()
    {
        Cursor.visible = false;
        gm.ToggleFakeCursor();
    }

    private void OnMouseDown()
    {
        if (!activated)
        {
            if (gm.activeUnit)
            {
                
                if (myType == ButtonType.move)
                {
                    gm.ShowTilesInMoveReach();
                }
                if (myType == ButtonType.attack)
                {
                    gm.ShowTilesInAttackReach();
                }
                if (myType == ButtonType.item)
                {
                    gm.GivePotion();
                    gm.unitLocked = true;
                }
                if(myType == ButtonType.ability)
                {
                    if(gm.activeUnit.mana > 0) {
                        gm.ShowTilesInSpellReach();
                        gm.unitLocked = true;
                    }
                }
            }
            if (myType == ButtonType.end)
            {
                gm.EndTurn();
                Cursor.visible = false;
                gm.ToggleFakeCursor();
            }
        }
    }

}
