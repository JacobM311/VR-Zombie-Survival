using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabManager : MonoBehaviour
{
    private Dictionary<GameObject, int> grabCounts = new Dictionary<GameObject, int>();

    // Singleton instance
    public static GrabManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void IncrementGrab(GameObject obj)
    {
        if (!grabCounts.ContainsKey(obj))
        {
            grabCounts[obj] = 0;
        }
        grabCounts[obj]++;
    }

    public void DecrementGrab(GameObject obj)
    {
        if (grabCounts.ContainsKey(obj))
        {
            grabCounts[obj]--;
            if (grabCounts[obj] == 0)
            {
                grabCounts.Remove(obj);
            }
        }
    }

    public int GetGrabCount(GameObject obj)
    {
        if (grabCounts.ContainsKey(obj))
        {
            return grabCounts[obj];
        }
        return 0;
    }
}