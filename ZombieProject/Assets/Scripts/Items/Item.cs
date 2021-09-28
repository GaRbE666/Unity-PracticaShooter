﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private float timeToDisappear;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip itemVoice;
    [SerializeField] private AudioSource powerUpAudioSource;
    [SerializeField] private AudioClip spawnPowerUp;
    [SerializeField] private AudioClip effectEnds;
    [SerializeField] private AudioClip pickPowerUp;
    [SerializeField] private GameObject nukeExplosion;

    public ItemScriptable itemScriptable;

    private bool itemCatched;

    //private MeshRenderer[] meshRenderer;
    private BoxCollider _boxCollider;
    private Text _powerUpTimerEffect;
    private float _timeToDestroy;
    private PlayerAudio _playerAudio;
    //private PlayerItemManager playerItemManager;
    private EnemyPowerUpManager[] _enemyPowerUpManager;
    [HideInInspector] public bool instakillActived;
    [HideInInspector] public bool doublePointsActived;

    private void Awake()
    {
        //playerItemManager = FindObjectOfType<PlayerItemManager>();
        //meshRenderer = GetComponentsInChildren<MeshRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
        _powerUpTimerEffect = FindObjectOfType<PowerUpTimer>().GetComponent<Text>();
        _playerAudio = FindObjectOfType<PlayerAudio>();
    }

    private void Start()
    {
        _timeToDestroy = itemScriptable.timeToDestroy;
        powerUpAudioSource.PlayOneShot(spawnPowerUp);
    }

    private void Update()
    {
        if (itemCatched)
        {
            
            _timeToDestroy -= Time.deltaTime;
            UpdatePowerUpText();
            if (_timeToDestroy <= 0)
            {
                Destroy(gameObject, 1f);
            }

        }
        else
        {
            timeToDisappear -= Time.deltaTime;
            
            if (timeToDisappear <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void UpdatePowerUpText()
    {
        _powerUpTimerEffect.enabled = true;
        _powerUpTimerEffect.text = _timeToDestroy.ToString("00");
        if (_timeToDestroy <= 0)
        {
            _powerUpTimerEffect.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            itemCatched = true;
            _boxCollider.enabled = false;
            DisableVisualPowerUp();
            powerUpAudioSource.PlayOneShot(pickPowerUp);
            //Debug.Log(powerUpAudioSource.clip.name);
            PlayItemVoice();
            SelectItem(other);
            FindAllZombiePowerUpManagerReferences();
        }
    }

    private void DisableVisualPowerUp()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == transform.childCount - 1)
            {
                return;
            }
            transform.GetChild(i).gameObject.SetActive(false);
        }
        
    }

    private void FindAllZombiePowerUpManagerReferences()
    {
        _enemyPowerUpManager = FindObjectsOfType<EnemyPowerUpManager>();
        foreach (EnemyPowerUpManager EPUM in _enemyPowerUpManager)
        {
            EPUM.StoreItem();
        }
    }

    private void SelectItem(Collider other)
    {
        switch (itemScriptable.itemType)
        {
            case ItemScriptable.itemEnumType.MaxAmmo:
                MaxAmmo(other);
                break;
            case ItemScriptable.itemEnumType.InstaKill:
                instakillActived = true;
                transform.name = "InstakillActived";
                StartCoroutine(InstaKill());
                break;
            case ItemScriptable.itemEnumType.DoublePoints:
                doublePointsActived = true;
                transform.name = "DoublePointsActived";
                StartCoroutine(DoublePoints());
                break;
            case ItemScriptable.itemEnumType.Cash:
                Cash(other);
                break;
            case ItemScriptable.itemEnumType.Kaboom:
                Kaboom();
                break;

        }
    }

    private void Kaboom()
    {
        _playerAudio.PlayKaboomAudio();
        Instantiate(nukeExplosion, transform.position, Quaternion.identity);
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        List<EnemyHealth> enemiesAlive = new List<EnemyHealth>();

        foreach (EnemyHealth enemy in enemies)
        {
            if (!enemy.die)
            {
                enemiesAlive.Add(enemy);
            }
        }
        Debug.Log(enemiesAlive.Count);
        foreach (EnemyHealth enemy in enemiesAlive)
        {
            StartCoroutine(enemy.Die());
        }
    }

    private void MaxAmmo(Collider other)
    {
        _playerAudio.PlayMaxAmmoAudio();
        other.GetComponent<PlayerItemManager>().MaxAmmo();
    }

    private IEnumerator InstaKill()
    {
        _playerAudio.PlayInstaKillAduio();
        yield return new WaitForSeconds(itemScriptable.timeToDestroy);
        powerUpAudioSource.PlayOneShot(effectEnds);
        instakillActived = false;
        EnemyHealth[] enemies2 = FindObjectsOfType<EnemyHealth>();
        foreach (EnemyHealth enemy in enemies2)
        {
            if (!enemy.GetComponent<EnemyHealth>().die)
            {
                enemy.GetComponent<EnemyHealth>().currentHealth = enemy.GetComponent<EnemyHealth>().zScriptable.maxHealth;
            }
        }
    }

    private IEnumerator DoublePoints()
    {
        _playerAudio.PlayDobulePointsAudio();
        yield return new WaitForSeconds(itemScriptable.timeToDestroy);
        powerUpAudioSource.PlayOneShot(effectEnds);
        EnemyIA[] enemies2 = FindObjectsOfType<EnemyIA>();
        doublePointsActived = false;
        foreach (EnemyIA enemy in enemies2)
        {
            if (enemy != null && !enemy.GetComponent<EnemyHealth>().die)
            {
                enemy.GetComponent<EnemyIA>().pointsReward /= 2;
                enemy.GetComponent<EnemyIA>().pointsForHitReward /= 2;
            }
        }
    }

    private void Cash(Collider other)
    {
        other.GetComponent<PlayerScore>().AddScore(1000);
        Destroy(gameObject);
    }

    private void PlayItemVoice()
    {
        audioSource.PlayOneShot(itemVoice);
    }

}
