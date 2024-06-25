
using UnityEngine;

public static class Utils{
    /// <summary>
    /// Destroys all children of a transform
    /// </summary>
    /// <param name="parent"></param>
    public static void DestroyAllChildren(this Transform parent){
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject.Destroy(parent.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Destroys all children of a transform immediately (use in editor only)
    /// </summary>
    /// <param name="parent"></param>
    public static void DestroyAllChildrenImmediate(this Transform parent){
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject.DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}