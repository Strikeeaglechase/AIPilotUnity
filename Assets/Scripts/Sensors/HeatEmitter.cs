using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatEmitter : ActorBehaviour
{
    public static List<HeatEmitter> emitters = new List<HeatEmitter>();
    public bool isMissile;
    public bool isCountermeasure;
    public float heat;

    public float cooldownRate;
    private float finalCooldown = 0.0001f;

    private Vector3 _vel;
    public Vector3 velocity
    {
        get
        {
            if (!actor)
            {
                return _vel;
            }
            return actor.velocity;
        }
        set
        {
            _vel = value;
        }
    }

    public void AddHeat(float addHeat)
    {
        heat += addHeat;
    }

    public void SetCooldownRate(float r)
    {
        cooldownRate = r;
        finalCooldown = 0.014f * cooldownRate;
    }

    void Start()
    {
        SetCooldownRate(cooldownRate);
    }

    void FixedUpdate()
    {
        heat = Mathf.Lerp(heat, 0, Mathf.Min(0.5f, finalCooldown * Time.fixedDeltaTime));
    }

    private void OnEnable()
    {
        emitters.Add(this);
    }

    private void OnDisable()
    {
        emitters.Remove(this);
    }
}
