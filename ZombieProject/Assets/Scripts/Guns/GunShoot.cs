﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunShoot : MonoBehaviour
{
    [Header("GameObjects References")] 
    public GunScriptable gunScriptable;
    [SerializeField] private GameObject muzzleParticles;
    [SerializeField] private Transform muzzlePosition;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GameObject headExplodeEffect;
    [Header("Texts References")]
    [SerializeField] private Text gunNameText;
    [SerializeField] private Text noAmmoText;

    private Text _chargeAmmoText;
    private Text _bedroomAmmoText;


    [HideInInspector] public bool isShooting;
    [HideInInspector] public bool isRealoading;
    [HideInInspector] public int currentChargerAmmo;
    [HideInInspector] public int currentBedroomAmmo;
    public float reloadTime;
    [HideInInspector] public float fireRate;

    private float _nextTimeToFire;
    private PlayerMovement _playerMovement;
    private Camera _cam;

    private void Awake()
    {
        _playerMovement = FindObjectOfType<PlayerMovement>();
        _chargeAmmoText = FindObjectOfType<chargerAmmo>().GetComponent<Text>();
        _bedroomAmmoText = FindObjectOfType<bedroomAmmo>().GetComponent<Text>();
        _cam = Camera.main;
    }

    private void Start()
    {
        InitializeVariables();
        InitializeAmmo();
    }

    private void InitializeVariables()
    {
        reloadTime = gunScriptable.reloadingtime;
        fireRate = gunScriptable.fireRate;
    }

    private void InitializeAmmo()
    {
        currentBedroomAmmo = gunScriptable.maxBulletsInBedroom;
        currentChargerAmmo = gunScriptable.maxBulletPerCharger;
        UpdateAmmoTexts();
    }

    private void OnEnable()
    {
        UpdateAmmoTexts();
        isRealoading = false;
        gunNameText.text = gunScriptable.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRealoading)
        {
            return;
        }
        ReloadChecker();
        ShootGun();
    }

    private void ReloadChecker()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentChargerAmmo <= 0 || currentChargerAmmo < gunScriptable.maxBulletPerCharger)
            {
                if (!_playerMovement.isRunning && currentBedroomAmmo > 0)
                {
                    Reload();
                }

            }
        }
    }

    private void ShootGun()
    {
        if (currentChargerAmmo != 0)
        {
            if (gunScriptable.singleShoot)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    isShooting = true;
                    isRealoading = false;
                    Shoot();
                }
                else
                {
                    isShooting = false;
                }
            }
            else
            {
                if (Input.GetButton("Fire1") && Time.time >= _nextTimeToFire)
                {
                    isShooting = true;
                    isRealoading = false;
                    _nextTimeToFire = Time.time + 1f / gunScriptable.fireRate;
                    Shoot();
                }
                else
                {
                    isShooting = false;
                }
            }
        }
        else
        {
            isShooting = false;
        }

    }

    private void Reload()
    {
        isShooting = false;
        StartCoroutine(ReloadingCoroutine());
        return;
    }

    private IEnumerator ReloadingCoroutine()
    {
        isRealoading = true;
        currentBedroomAmmo -= (gunScriptable.maxBulletPerCharger - currentChargerAmmo);
        yield return new WaitForSeconds(reloadTime);
        currentChargerAmmo = gunScriptable.maxBulletPerCharger;
        isRealoading = false;
        UpdateAmmoTexts();
    }

    private void Shoot()
    {
        SpendAmmo();
        ThrowRay();
    }

    private void ThrowRay()
    {
        RaycastHit hit;
        GameObject muzzleClone = Instantiate(muzzleParticles, muzzlePosition.transform.position, transform.rotation);
        //muzzleClone.transform.SetParent(muzzlePosition);
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, gunScriptable.range))
        {
            if (hit.collider.gameObject.layer == 9)
            {
                CalculateZombieDamage(hit);
            }
        }
        Debug.DrawRay(_cam.transform.position, _cam.transform.forward, Color.green, gunScriptable.range);
    }

    private void CalculateZombieDamage(RaycastHit hit)
    {
        EnemyHealth enemy = hit.collider.gameObject.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(gunScriptable.damage);
        }
        SelectWhatParticleShow(hit, enemy);

    }

    private void SelectWhatParticleShow(RaycastHit hit, EnemyHealth enemy)
    {
        if (hit.collider.gameObject.name.Equals("HeadCollider") && enemy.currentHealth <= 0)
        {
            GameObject impactHeadClone = Instantiate(headExplodeEffect, hit.collider.gameObject.transform.position, headExplodeEffect.transform.rotation);
            impactHeadClone.transform.SetParent(hit.transform);
            GameObject.Find("Z_Head").SetActive(false);
            hit.collider.enabled = false;
        }
        else
        {
            GameObject impactClone = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            impactClone.transform.SetParent(hit.transform);
        }
    }

    private void SpendAmmo()
    {
        currentChargerAmmo--;

        UpdateAmmoTexts();
    }

    public void UpdateAmmoTexts()
    {

        if (currentChargerAmmo < 10 && !gunScriptable.singleShoot)
        {
            noAmmoText.text = "Reload...";
            noAmmoText.color = Color.yellow;
        }
        
        if(currentChargerAmmo == 0)
        {
            noAmmoText.text = "No ammo";
            noAmmoText.color = Color.red;
        }
        else
        {
            noAmmoText.text = "";
        }

        if (currentChargerAmmo <= 0)
        {
            currentChargerAmmo = 0;
        }
        if (currentBedroomAmmo < 0)
        {
            currentBedroomAmmo = 0;
        }
        _chargeAmmoText.text = currentChargerAmmo.ToString();
        _bedroomAmmoText.text = currentBedroomAmmo.ToString();
    }
}
