// Unity Only File

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI leftStack;
    public TextMeshProUGUI speed;


    private Rigidbody rb;
    public KinematicPlane kplane;

    public Radar radar;
    public RectTransform hudRadarLock;

    public void Start()
    {
        rb = kplane.GetComponent<Rigidbody>();
    }

    public void Update()
    {
        var alt = kplane.transform.position.y.ToString("N0");
        var gForce = (kplane.acceleration.magnitude / Physics.gravity.magnitude).ToString("N1");
        var kpSpeed = rb.linearVelocity.magnitude.ToString("N0");
        var throttle = kplane.engine.throttle.ToString("N2");

        leftStack.SetText($"{alt}\n{gForce}");
        speed.SetText($"{kpSpeed}\n{throttle}");

        if (radar.lockData != null)
        {
            hudRadarLock.gameObject.SetActive(true);

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(radar.lockData.position);

            float markerX = ((screenPosition.x / Screen.width) - 0.5f) * ((RectTransform)transform).rect.width;
            float markerY = ((screenPosition.y / Screen.height) - 0.5f) * ((RectTransform)transform).rect.height;

            hudRadarLock.anchoredPosition = new Vector2(markerX, markerY);
        }
        else
        {
            hudRadarLock.gameObject.SetActive(false);
        }
    }
}
