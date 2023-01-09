using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using CMF;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    [Header("InitialStatistics")]
    [SerializeField] private float _initialHealth;
    [SerializeField] private float _initialOxygen;

    [Space(10)] [Header("DecreaseValue")] 
    [SerializeField] private float _decreaseHealth;
    [SerializeField] private float _decreaseOxygen;

    [Space(10)] [Header("IncreaseValue")]
    [SerializeField] private float _increaseOxygen;

    [Space(10)][Header("References")]
    [SerializeField] private Animator _animator;

    [SerializeField] private PlayerAudio _playerAudio;

    [SerializeField] private GameObject _dieObject;

    private PlayerGameIdentity _player;
    private StatisticsPanel _statisticsPanel;
    
    private float _currHealth;
    private float _currOxygen;

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

        if (_isInBase)
        {
            if(_currOxygen < _initialOxygen) _currOxygen += _increaseOxygen * Time.deltaTime;
        }
        else
        {
            _currOxygen -= _decreaseOxygen * Time.deltaTime;
        }

        if (_currOxygen <= 0) _currHealth -= _decreaseHealth * Time.deltaTime;

        if (_currHealth <= 0)
        {
            Death();
        }
    }

    public void TakeDamage(float value)
    {
        _currHealth -= value;
        _playerAudio.PlayHit();
    }
    
    public void Death()
    {
        _animator.SetBool(Die, true);

        _isDead = true;
        
        _player.EnableInput(false);
        
        NetworkManager.Instance.ClientMessages.SendDeath();

        RessourceManager.Instance.Death(_player.GetId);
        
        _playerAudio.PlayDieSound();
        
        StartCoroutine(Respawn());

        StartCoroutine(PlayDieEffect());
    }

    private IEnumerator PlayDieEffect()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject dieInstance = Instantiate(_dieObject, transform.position, transform.GetChild(0).rotation);
        Destroy(dieInstance, 3f);
    }
    
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(7.5f);

        _animator.SetBool(Die, false);
        
        SetupStatistics();
        UpdateInterface();
        
        GetComponent<AdvancedWalkerController>().momentum = Vector3.zero;
        
        transform.position = GameManager.Instance.GetRespawnPos(_player.TeamId);

        _player.EnableInput(true);

        _player.Aim.ReloadInstante();
        
        _isDead = false;
    }   
    
    public void SetupStatistics()
    {
        _isInBase = true;
        
        _currHealth = _initialHealth;
        _currOxygen = _initialOxygen;
    }
    
    private void UpdateInterface()
    {
        _statisticsPanel.SetBarValue(_currHealth / _initialHealth, _currOxygen / _initialOxygen);
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