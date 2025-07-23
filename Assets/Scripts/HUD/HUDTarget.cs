// Unity Only File

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDTarget : MonoBehaviour
{
    public void Start()
    {
        HUDMarkerManager.instance.CreateMarker(gameObject, "\n\n");
    }
}
