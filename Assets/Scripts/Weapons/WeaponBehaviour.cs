using System;
using System.Collections;
using System.Collections.Generic;
using TPSDemo.Characters;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour, iPoolable
{
    public Transform myFirePosition;
    

    public Action OnClipSizeChange;
    public Action<int> OnBulletConsumed;
    public Action OnBulletFired;
    public Action<Vector3> OnBulletHit;

    WeaponController myController;
    float damage;
    int clipSize;
    float fireRate;
    LayerMask layerMask;

    public WeaponController MyController
    {
        get => myController;
        private set 
        {
            myController = value;

            if(myController)
                myController.OnFireInput += Fire;
        }
    }


    float internalFireRate = 0.0f;
    int currentClip = 0;
    public int CurrentClip
    {
        get => currentClip;
        set 
        {
            currentClip = value;

            if (OnBulletConsumed != null)
                OnBulletConsumed.Invoke(value);
        }
    }


    public void Setup(float damage , int clipSize , float fireRate , LayerMask layerMask , WeaponController myController)
    {
        this.damage = damage;
        this.clipSize = clipSize;
        this.fireRate = fireRate;
        this.MyController = myController;
        this.layerMask = layerMask;

        CurrentClip = clipSize;
    }

    private void Fire() 
    {
        if (!CanShoot()) return;

        Debug.Log("Fire!");

        RaycastHit hitInfo;
        if (Physics.Raycast(myFirePosition.position , myFirePosition.forward , out hitInfo , 200.0f, layerMask)) 
        {
            if (hitInfo.collider.CompareTag("Enemy")) 
            {
                hitInfo.collider.GetComponent<EnemyCharacter>().RecieveDamage(damage);
            }

            if (OnBulletHit != null)
                OnBulletHit.Invoke(hitInfo.point);
        }

        internalFireRate = fireRate;
        StartCoroutine(fireRateCooldown());

        CurrentClip--;
    }

    public void Reload() 
    {
        CurrentClip = clipSize;

        Debug.Log("Reload!");
    }

    private bool CanShoot() 
    {
        if (CurrentClip <= 0) return false;

        if (internalFireRate > 0.0f) return false;

        return true;
    }

    IEnumerator fireRateCooldown() 
    {
        while (internalFireRate > 0.0f)
        {
            internalFireRate -= Time.deltaTime;
            yield return null;
        }
    }

    public void OnPool()
    {
        myController.OnFireInput -= Fire;
        myController = null;
        damage = 0.0f;
        clipSize = 0;
        fireRate = 0.0f;
    }

    public void OnUnpool()
    {
    }
}
