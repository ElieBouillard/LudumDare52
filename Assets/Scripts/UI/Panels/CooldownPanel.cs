using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _cooldownText;
    [SerializeField] private Image _cooldownImage;
    [SerializeField] private float _cooldownTime;

    private float _cooldownClock;

    public bool GameIsStarted = false;
    
    public void StartCouldown()
    {
        _cooldownImage.fillAmount = 1;
        _cooldownClock = _cooldownTime;
    }

    private void Update()
    {
        if (_cooldownClock >= 0)
        {
            _cooldownClock -= Time.deltaTime;
            _cooldownImage.fillAmount = _cooldownClock / _cooldownTime;
            _cooldownText.text = _cooldownClock.ToString("0.0");
        }
        else if(_cooldownClock != -1)
        {
            _cooldownClock = -1;
            ((PlayerGameIdentity) NetworkManager.Instance.LocalPlayer).EnableInput(true);
            GameIsStarted = true;
            gameObject.SetActive(false);
        }
    }
}
