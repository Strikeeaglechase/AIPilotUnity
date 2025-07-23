using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HCMissile : HCEntityBase
{
    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;

    public bool fired = false;
    public Color trailColor = Color.white;
    private Vector3 prevPosition;

    private WeaponStats.WeaponInfo info;
    public float rcs => info.rcs;

    private void Start()
    {
        var weaponInfo = LocalFightController.instance.weaponStats.weaponPrefabs.Find(p => p.path == path);
        if (weaponInfo != null)
        {
            info = weaponInfo;
        }
        else
        {
            Debug.LogError($"Unable to resolve weapon stats for {path} ({entityId})");
        }
    }

    private void FixedUpdate()
    {
        velocity += acceleration * Time.fixedDeltaTime;
        transform.position += velocity * Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (!fired) return;

        if ((transform.position - prevPosition).sqrMagnitude > 100 * 100)
        {
            prevPosition = transform.position;
            return;
        }

        Debug.DrawLine(transform.position, prevPosition, trailColor, 10);
        prevPosition = transform.position;
    }
}
