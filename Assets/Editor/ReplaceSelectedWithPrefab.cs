using UnityEditor;
using UnityEngine;

public class ReplaceSelectedWithPrefab : EditorWindow
{
    private GameObject prefab;

    [MenuItem("Tools/Replace Selected With Prefab")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceSelectedWithPrefab>("Replace With Prefab");
    }

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace Selected Objects"))
        {
            ReplaceSelected();
        }
    }

    private void ReplaceSelected()
    {
        if (prefab == null)
        {
            Debug.LogError("No prefab assigned.");
            return;
        }

        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject oldObject in selectedObjects)
        {
            Transform oldTransform = oldObject.transform;

            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefab");

            newObject.name = oldObject.name;

            Transform newTransform = newObject.transform;
            newTransform.SetParent(oldTransform.parent);
            newTransform.SetSiblingIndex(oldTransform.GetSiblingIndex());

            newTransform.position = oldTransform.position;
            newTransform.rotation = oldTransform.rotation;
            newTransform.localScale = oldTransform.localScale;

            Undo.DestroyObjectImmediate(oldObject);
        }
    }
}
