using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDistantHealth : MonoBehaviour
{
    [SerializeField] private float _initialHealth;
    [SerializeField] private Image _healthBarImage;
    
    private float _currHealth;

    private void Awake()
    {
        InitializeHealth();
    }

    public void InitializeHealth()
    {
        _currHealth = _initialHealth;
        _healthBarImage.fillAmount = _currHealth / _initialHealth;
    }
    
    public void TakeDamage(float damage)
    {
        _currHealth -= damage;
        _healthBarImage.fillAmount = _currHealth / _initialHealth;
    }
}
