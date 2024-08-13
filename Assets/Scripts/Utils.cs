
using System;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Destroys all children of a transform
    /// </summary>
    /// <param name="parent"></param>
    public static void DestroyAllChildren(this Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject.Destroy(parent.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Disables all direct children of a transform
    /// </summary>
    /// <param name="parent"></param>
    public static void DisableAllChildren(this Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Gets the current time in epoch ms
    /// </summary>
    /// <returns>Returns the current time in epoch ms</returns>
    public static float GetCurrentTime()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    private static float easeInOutCubic(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
    public static float CubicEase(float min, float max, float t)
    {
        // Cubic easing function: t^3
        float easedT = easeInOutCubic(t);
        return Mathf.Lerp(min, max, easedT);
    }
}

public class MonoBehavour
{
}