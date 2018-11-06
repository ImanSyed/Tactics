using UnityEngine;
using UnityEditor;


public class ToolScript{

    [MenuItem("Tools/AssignScript")]
    public static void AssignScript()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("TIles");
        foreach(GameObject t in tiles)
        {
            if (t.GetComponent<WorldTile>() == null)
            {
                t.AddComponent<WorldTile>();
            }
        }
    }
}
