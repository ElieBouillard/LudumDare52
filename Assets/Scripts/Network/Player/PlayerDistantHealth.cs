using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDistantHealth : MonoBehaviour
{
    [SerializeField] private float _initialHealth;
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private Color[] _colors;

    private float _currHealth;

    private void Start()
    {
        InitializeHealth();

        _healthBarImage.color = NetworkManager.Instance.LocalPlayer.TeamId == GetComponent<PlayerIdentity>().TeamId ? _colors[0] : _colors[1];
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

        if (_currHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        RessourceManager.Instance.Death(GetComponent<PlayerIdentity>().GetId);
    }
}
