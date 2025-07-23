using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/WeaponStatsScriptableObject", order = 1)]
public class WeaponStats : ScriptableObject
{
    [Serializable]
    public class WeaponInfo
    {
        public GameObject prefab;
        public string path;
        public float rcs;
        public float mass;
        public float drag;
    }

    public List<WeaponInfo> weaponPrefabs = new List<WeaponInfo>();

}
