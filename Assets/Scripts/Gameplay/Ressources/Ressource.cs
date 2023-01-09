using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class Ressource : MonoBehaviour
{
    [SerializeField] private int _initialHealth = 3;
    [SerializeField] private int _value = 1;
    
    [SerializeField] private float _speedWhenCollected;
    
    private Collider _collider;
    private Outline _outline;
    private GameObject _children;
    
    public RessourceType RessourceType = RessourceType.Fer;
    
    public ushort Id;
    
    private int _currHealth;

    private bool _isInTravel;
    private bool _isInTravelBack;
    public bool IsCollected { private set; get; }
    public bool IsCollectedInBase { private set; get; }

    [SerializeField] private AudioSource _breakAudioSource;
    [SerializeField] private AudioSource _collectAudioSource;

    private Vector3 _startPos;
    
    public PlayerIdentity PlayerCollector { private set; get; }

    private float _initialDistance;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _outline = GetComponentInChildren<Outline>();
        _children = transform.GetChild(0).gameObject;
        
        _currHealth = _initialHealth;
        
        _outline.OutlineWidth = 0f;

        _startPos = transform.position;
    }

    private void Update()
    {
        if (_isInTravel && !IsCollected)
        {
            transform.position += (PlayerCollector.transform.position + Vector3.up - transform.position).normalized * _speedWhenCollected * Time.deltaTime;
            transform.Rotate(new Vector3(180f, 180f, 0) * Time.deltaTime);

            float currDistance = (PlayerCollector.transform.position + Vector3.up - transform.position).magnitude;
        
            transform.localScale = Vector3.one * (currDistance  / _initialDistance);

            if (currDistance <= 0.5f)
            {
                Collected();
            }
        }

        if (_isInTravelBack)
        {
            transform.position += (_startPos - transform.position).normalized * _speedWhenCollected * Time.deltaTime;
            transform.Rotate(new Vector3(180f, 180f, 0) * Time.deltaTime);
            float currDistance = (_startPos - transform.position).magnitude;
            transform.localScale = Vector3.one * (1 - currDistance  / _initialDistance);
            
            if (currDistance <= 0.5f)
            {
                SetupToStartPos();
            }
        }
    }

    public void TakeDamage(ushort playerId, int damage = 1)
    {
        if (_isInTravel) return;
        
        _currHealth -= damage;

        if (_currHealth <= 0)
        {
            InitializeTravelToPlayer(playerId);
            NetworkManager.Instance.ClientMessages.SendRessourceTravel(Id);
        }
    }

    public void InitializeTravelToPlayer(ushort playerId)
    {
        EnableOutline(false);

        _breakAudioSource.Play();
        
        PlayerCollector = NetworkManager.Instance.Players[playerId];

        _initialDistance = (PlayerCollector.transform.position - transform.position).magnitude;

        _collider.enabled = false;
        
        _isInTravel = true;
    }

    public void InitializeTravelToStartPos()
    {
        transform.position = PlayerCollector.transform.position + Vector3.up;
        _initialDistance = (_startPos - transform.position).magnitude;
        _isInTravelBack = true;
        _children.SetActive(true);
    }

    private void SetupToStartPos()
    {
        PlayerCollector = null;
        _isInTravel = false;
        _collider.enabled = true;
        _isInTravelBack = false;
        IsCollected = false;

        _currHealth = _initialHealth;
        
        transform.DOMove(_startPos, 0.2f).SetEase(Ease.Linear);
        transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.Linear);
    }
    
    public void Collected()
    {
        PlayerIdentity player = PlayerCollector.GetComponent<PlayerIdentity>();
        
        RessourceManager.Instance.AddRessourceToPlayer(player.GetId, RessourceType, 1);

        _collectAudioSource.Play();
        
        _children.SetActive(false);

        _isInTravel = false;
        
        IsCollected = true;
    }

    public void CollectedToBase()
    {
        IsCollectedInBase = true;
    }
    
    public void EnableOutline(bool value)
    {
        if (_isInTravel) return;
        _outline.OutlineWidth = value ? 2f : 0f;
    }
}

public enum RessourceType
{
    Fer,
    Plastic,
    Energy
}
