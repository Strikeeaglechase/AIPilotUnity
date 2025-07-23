using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public struct IRWeaponState
{
    public float seekerFov;
    public float heat;
    public NetVector lookDir;

    public IRWeaponState(HeatSeeker seeker)
    {
        seekerFov = seeker.seekerFOV;
        heat = seeker.seenHeat;
        lookDir = new NetVector(seeker.transform.forward);
    }
}

public class EquipManager : MonoBehaviour, IVehicleReadyNotificationHandler, IWeaponRCSProvider
{
    public List<string> equips = new List<string>();
    public List<Missile> weapons = new List<Missile>();
    public WeaponStats weaponStats;
    public Radar radar;
    public SimpleDrag equipDrag;

    public int selectedWeapon = 0;

    public bool tryFire = false;

    public void OnVehicleReadyNotification()
    {
        for (int i = 0; i < equips.Count; i++)
        {
            if (string.IsNullOrEmpty(equips[i])) continue;

            var equipSpawnCommand = new SpawnAIPWeapon
            {
                path = equips[i],
                hpIndex = i
            };

            Debug.Log($"Requesting spawn {equips[i]} onto hp {i}");
            HCConnector.instance.SendCommandPacket(equipSpawnCommand);
            LocalFightController.instance.RequestWeaponSpawn(this, equipSpawnCommand);
        }
    }

    public void HandleEquipSpawned(string equipPath, int entityId, int hpIndex)
    {
        Debug.Log($"Equip spawned: {equipPath} ({entityId}) on hp {hpIndex}");
    }

    public void HandleWeaponSpawned(string weaponPath, int weaponEntityId, int hpEntityId, int railIndex)
    {
        Debug.Log($"Weapon {weaponPath} spawned with id {weaponEntityId}, spawned onto {hpEntityId} rail idx {railIndex}");
        var info = weaponStats.weaponPrefabs.Find(p => p.path == weaponPath);
        if (info == null)
        {
            Debug.LogError($"Unable to locate prefab for {weaponPath}");
            return;
        }

        //Debug.Log($"Weapon info: {info}, prefab: {wea}");

        var missileGo = Instantiate(info.prefab);
        missileGo.transform.SetParent(transform);
        missileGo.transform.localPosition = Vector3.zero;
        missileGo.transform.localRotation = Quaternion.identity;

        var missileComp = missileGo.GetComponent<Missile>();
        missileComp.entityId = weaponEntityId;
        missileComp.weaponPath = weaponPath;

        weapons.Add(missileComp);

        UpdateEquipDrag();
    }

    private void FireNextARHMissile(int pdt = -1)
    {
        var arh = weapons.Find(w => w != null && w.GetComponent<Missile>().guidanceMode == MissileGuidanceMode.Radar);
        if (arh == null)
        {
            Debug.LogWarning($"Attempt to fire ARH Missile however no radar missiles found");
            return;
        }

        var missile = arh.GetComponent<Missile>();
        var selectedTarget = (pdt != -1 && pdt > 0 && pdt < radar.twsedTargets.Count) ? radar.twsedTargets[pdt] : radar.lockData?.actor;
        if (selectedTarget == null)
        {
            //Debug.LogWarning($"No radar target in fire ARH missile");
            return;
        }

        tryFire = false;
        Debug.Log($"Firing ARH missile {missile} at {selectedTarget}");
        missile.FireWithRadarData(radar, selectedTarget);
        weapons.Remove(arh);

        UpdateEquipDrag();
    }

    private Missile GetSelectedWeapon()
    {
        if (selectedWeapon >= 0 && selectedWeapon < weapons.Count) return weapons[selectedWeapon];
        return null;
    }

    public void FireSelectedMissile()
    {
        var missile = GetSelectedWeapon();

        if (missile == null)
        {
            Debug.LogWarning($"Attempt to fire had selected {selectedWeapon} which is not a valid weapon index");
            return;

        }

        if (missile.guidanceMode == MissileGuidanceMode.Radar)
        {
            var selectedTarget = (radar.pdtTwsIdx != -1 && radar.pdtTwsIdx > 0 && radar.pdtTwsIdx <= radar.twsedTargets.Count) ? radar.twsedTargets[radar.pdtTwsIdx] : radar.lockData?.actor;
            if (selectedTarget == null)
            {
                Debug.LogWarning($"No selected target for ARH fire, pdt idx: {radar.pdtTwsIdx}");
                return;
            }

            missile.FireWithRadarData(radar, selectedTarget);
            weapons.Remove(missile);
        }
        else
        {
            missile.Fire();
            weapons.Remove(missile);
        }

        UpdateEquipDrag();
    }

    private void UpdateEquipDrag()
    {
        float totalDrag = 0;
        foreach (var weapon in weapons)
        {
            totalDrag += weapon.info.drag;
        }

        equipDrag.area = totalDrag;
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        //weapons.RemoveAll(weapon => weapon == null || weapon.fired);
        if (tryFire && weapons.Count > 0)
        {

            FireNextARHMissile();
            //tryFire = false;
            //var missile = weapons[0].GetComponent<Missile>();
            //missile.Fire();
            //weapons.RemoveAt(0);
        }
    }

    private HeatSeeker GetSelectedWeaponHeatSeeker()
    {
        var weapon = GetSelectedWeapon();
        if (weapon == null) return null;

        var heatSeeker = weapon.heatSeeker;
        return heatSeeker;
    }

    public IRWeaponState GetIRWeaponState()
    {
        var heatSeeker = GetSelectedWeaponHeatSeeker();
        if (heatSeeker == null) return default(IRWeaponState);
        return new IRWeaponState(heatSeeker);
    }

    public void SetIRLookDir(Vector3 dir)
    {
        var heatSeeker = GetSelectedWeaponHeatSeeker();
        if (heatSeeker == null) return;

        heatSeeker.commandLookDir = dir;
    }

    public void SetIRTrigUncage(bool uncage)
    {
        var heatSeeker = GetSelectedWeaponHeatSeeker();
        if (heatSeeker == null) return;

        heatSeeker.triggerUncaged = uncage;
    }

    public string[] GetAttachedWeapons()
    {
        return weapons.Select(w => w.weaponPath).ToArray();
    }

    public float GetWeaponRCS()
    {
        return weapons.Select(m => m.rcs).Sum();
    }
}
