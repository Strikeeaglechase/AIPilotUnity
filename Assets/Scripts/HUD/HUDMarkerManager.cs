// Unity Only File

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDMarkerManager : MonoBehaviour
{
    public GameObject markerPrefab;
    public Transform markerRoot;

    public static HUDMarkerManager instance { get; private set; }

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogError($"Duplicate HUDMarkerManager created");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void CreateMarker(GameObject go, string name)
    {
        var newMarker = Instantiate(markerPrefab, Vector3.zero, Quaternion.identity, markerRoot);
        newMarker.GetComponent<HUDMarker>().Bind(go, name);
    }
}
