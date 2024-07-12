
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

}

public class MonoBehavour
{
}