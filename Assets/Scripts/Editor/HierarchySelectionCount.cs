using UnityEngine;
using UnityEditor;

public class HierarchySelectionCount : Editor
{

    [MenuItem("GameObject/Get Selection Count", false, 0)]
    public static void  GetSelectionCount(MenuCommand menuCommand)
    {
        if (Selection.objects.Length > 1)
        {
            if (menuCommand.context != Selection.objects[0])//增加限制，只有第一个物体打印。
            {
                return;
            }
        }
        Debug.Log("Selection Gameobject Count===============" + Selection.objects.Length);
    }

}
