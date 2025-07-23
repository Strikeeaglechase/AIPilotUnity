using Recorder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRCountermeasure : ActorBehaviour
{
    public float flareLife = 7f;
    public GameObject flarePrefab;
    public float ejectSpeed = 45f;

    public int count = 120;
    public bool fireFlare = false;

    private Vector3 WeightedDirectionDeviation(Vector3 direction, float maxAngle)
    {
        float num = UnityEngine.Random.Range(0f, 1f);
        float maxRadiansDelta = maxAngle * (num * num) * ((float)Math.PI / 180f);
        return Vector3.RotateTowards(direction, Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, direction), maxRadiansDelta, 0f).normalized;
    }


    public bool FireFlare()
    {
        if (count <= 0) return false;
        count--;
        var fvelocity = actor.velocity + UnityEngine.Random.Range(ejectSpeed * 0.9f, ejectSpeed * 1.1f) * WeightedDirectionDeviation(-transform.up, 3f);

        var flare = Instantiate(flarePrefab, transform.position, Quaternion.LookRotation(UnityEngine.Random.onUnitSphere));

        var flareComp = flare.GetComponent<CMFlare>();
        flareComp.velocity = fvelocity;
        flareComp.flareLife = flareLife;

        GameRecorder.Event(new FlareEvent(actor.entityId));

        return false;
    }

    private void Update()
    {
        if (fireFlare)
        {
            FireFlare();
            fireFlare = false;
        }
    }
}
