﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerOn : MonoBehaviour
{
    [SerializeField] private Text powerText;
    [SerializeField] private Animation _animation;
    public delegate void Power();
    public event Power PowerOnReleased;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        powerText.gameObject.SetActive(false);
        SetPowerText("Mantén F para encender");
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        ActiveText();
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!_gameManager.powerOn)
            {
                ActiveText();
                PlayerPressKey();
            }
            else
            {
                DesactiveText();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DesactiveText();
        }
    }

    private void PlayerPressKey()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            PowerOnReleased?.Invoke();
            _animation.Play();
        }
    }

    private void SetPowerText(string text)
    {
        powerText.text = text;
    }

    private void ActiveText()
    {
        powerText.gameObject.SetActive(true);
    }

    private void DesactiveText()
    {
        powerText.gameObject.SetActive(false);
    }
}
