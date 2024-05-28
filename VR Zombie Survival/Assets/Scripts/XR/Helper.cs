using UnityEngine;
using System.Collections.Generic;

public static class Helper
{
    public static GameObject[] FindChildrenWithTags(this GameObject parent, string tag, bool includeInactive = false)
    {
        if (parent == null) throw new System.ArgumentNullException(nameof(parent));
        if (string.IsNullOrEmpty(tag)) throw new System.ArgumentNullException(nameof(tag));

        List<GameObject> list = new List<GameObject>();
        Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);
        foreach (Transform child in children)
        {
            if (child.CompareTag(tag))
            {
                list.Add(child.gameObject);
            }
        }
        return list.ToArray();
    }

    public static GameObject FindChildWithTag(this GameObject parent, string tag, bool includeInactive = false)
    {
        if (parent == null) throw new System.ArgumentNullException(nameof(parent));
        if (string.IsNullOrEmpty(tag)) throw new System.ArgumentNullException(nameof(tag));

        Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);
        foreach (Transform child in children)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        return null;
    }
}