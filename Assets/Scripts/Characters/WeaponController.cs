using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPSDemo.Weapons;
using System;

namespace TPSDemo.Characters
{
    public class WeaponController : MonoBehaviour
    {
        public Transform weaponAttachPoint;
        public Weapon currentWeaponData;
        public LayerMask layerMask;

        WeaponBehaviour equipedWeapon;
        public Transform firePosition;

        public Entity character;
        public Action OnFireInput;

        private void Awake()
        {
            if (!character)
                character = GetComponent<Entity>();

            equipedWeapon = ObjectPool.instance.GetObjectOfType(currentWeaponData.weaponPrefab.GetInstanceID() , weaponAttachPoint.position , weaponAttachPoint.rotation.eulerAngles , weaponAttachPoint).GetComponent<WeaponBehaviour>();
            equipedWeapon.Setup(currentWeaponData.weaponDamage , currentWeaponData.clipSize , currentWeaponData.fireRate , layerMask , this);
            firePosition = equipedWeapon.myFirePosition;
        }

        public void TriggerFire()
        {
            if (OnFireInput != null)
                OnFireInput.Invoke();
        }

        public void Reload() 
        {
            equipedWeapon.Reload();
        }
    }
}