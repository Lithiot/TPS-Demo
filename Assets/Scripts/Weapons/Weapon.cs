using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSDemo.Weapons
{

    [CreateAssetMenu(fileName = "New Weapon" , menuName = "Weapon" , order = 0)]
    public class Weapon : ScriptableObject
    {
        [Header("Weapon Stats")]
        public string weaponName;
        public float weaponDamage;
        public int clipSize;
        public float fireRate;

        [Header("Prefab")]
        public GameObject weaponPrefab;
    }
}