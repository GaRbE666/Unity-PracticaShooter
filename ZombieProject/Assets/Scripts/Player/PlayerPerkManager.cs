﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerkManager : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private GunShoot[] gunsShoots;
    private GunAnimations[] gunsAnimations;
    private bool quickReviveActive;
    private bool speedColaActive;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

    }

    private void Update()
    {
        if (quickReviveActive)
        {
            QuickReviveEffect();
        }

        if (playerHealth._isDie)
        {
            RemoveAllPerks();
        }
    }

    public void SelectPerk(Perk.TypeOfPerks perkType, GameObject perk)
    {
        switch (perkType)
        {
            case Perk.TypeOfPerks.QuickRevive:
                quickReviveActive = true;
                break;
            case Perk.TypeOfPerks.Juggernaut:
                break;
            case Perk.TypeOfPerks.SpeedCola:
                speedColaActive = true;
                SpeedColaEffect();
                break;
            case Perk.TypeOfPerks.DoubleTap:
                break;
            case Perk.TypeOfPerks.StamminUp:
                break;
        }
    }

    private void QuickReviveEffect()
    {
        playerHealth.timeToRecoverHP /= 2;
    }

    private void RestoreQuickReviveEffect()
    {
        playerHealth.timeToRecoverHP *= 2;
    }

    private void SpeedColaEffect()
    {
        gunsShoots = FindObjectsOfType<GunShoot>();
        gunsAnimations = FindObjectsOfType<GunAnimations>();
        foreach (GunShoot gun in gunsShoots)
        {
            gun.reloadTime /= 2;
            Debug.Log("normal speed: " + gun.GetComponent<GunAnimations>().animator.speed);
            gun.GetComponent<GunAnimations>().animator.speed *= 1.2f;
            Debug.Log("fastes speed: " + gun.GetComponent<GunAnimations>().animator.speed);
        }

    }

    private void RestoreSpeedColaEffect()
    {
        foreach (GunShoot gun in gunsShoots)
        {
            gun.reloadTime *= 2;
        }
    }

    private void RemoveAllPerks()
    {
        quickReviveActive = false;
        speedColaActive = false;
        RestoreQuickReviveEffect();
        RestoreSpeedColaEffect();
    }
}
