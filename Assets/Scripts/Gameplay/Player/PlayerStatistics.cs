using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    [Header("InitialStatistics")]
    [SerializeField] private float _initialHealth;
    [SerializeField] private float _initialOxygen;
    [SerializeField] private float _initialFood;
    [SerializeField] private float _initialWater;

    [Space(10)] [Header("DecreaseValue")] 
    [SerializeField] private float _decreaseHealth;
    [SerializeField] private float _decreaseOxygen;
    [SerializeField] private float _decreaseFood;
    [SerializeField] private float _decreaseWater;

    [Space(10)] [Header("IncreaseValue")] 
    [SerializeField] private float _increaseHealth;
    [SerializeField] private float _increaseOxygen;

    [Space(10)][Header("References")]
    [SerializeField] private Animator _animator;
    
    private PlayerGameIdentity _player;
    private StatisticsPanel _statisticsPanel;
    
    private float _currHealth;
    private float _currOxygen;
    private float _currFood;
    private float _currWater;

    private bool _isInBase;

    private bool _isDead;
    
    private static readonly int Die = Animator.StringToHash("Die");

    private void Start()
    {
        _player = GetComponent<PlayerGameIdentity>();
        _statisticsPanel = StatisticsPanel.Instance;
        
        SetupStatistics();
        UpdateInterface();
    }

    private void Update()
    {
        if (_isDead) return;
        
        UpdateInterface();
        
        // if(_currFood > 0) _currFood -= _decreaseFood * Time.deltaTime;
        // if(_currWater > 0) _currWater -= _decreaseWater * Time.deltaTime;

        if (_isInBase)
        {
            if(_currOxygen < _initialOxygen) _currOxygen += _increaseOxygen * Time.deltaTime;
        }
        else
        {
            _currOxygen -= _decreaseOxygen * Time.deltaTime;
        }

        if (_currOxygen <= 0) _currHealth -= _decreaseHealth * Time.deltaTime;
        // if (_currFood <= 0) _currHealth -= _decreaseHealth * Time.deltaTime;
        // if (_currWater <= 0) _currHealth -= _decreaseHealth * Time.deltaTime;

        if (_currHealth <= 0)
        {
            Death();
        }
    }

    public void TakeDamage(float value)
    {
        _currHealth -= value;
    }
    
    private void Death()
    {
        _animator.SetBool(Die, true);

        _isDead = true;
        
        _player.EnableInput(false);
        
        NetworkManager.Instance.ClientMessages.SendDeath();
        
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        _animator.SetBool(Die, false);
        
        SetupStatistics();
        UpdateInterface();
        
        transform.position = GameManager.Instance.GetRespawnPos();

        _player.EnableInput(true);

        _isDead = false;
    }   
    
    public void SetupStatistics()
    {
        _isInBase = true;
        
        _currHealth = _initialHealth;
        _currOxygen = _initialOxygen;
        // _currFood = _initialFood;
        // _currWater = _initialWater;
    }
    
    private void UpdateInterface()
    {
        _statisticsPanel.SetBarValue(_currHealth / _initialHealth, _currOxygen / _initialOxygen, _currFood / _initialFood, _currWater/ _initialWater );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Base>())
        {
            _isInBase = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Base>())
        {
            _isInBase = false;
        }
    }
}