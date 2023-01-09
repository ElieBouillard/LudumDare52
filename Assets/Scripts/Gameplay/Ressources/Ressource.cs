using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using DG.Tweening;
using UnityEngine;

public class Ressource : MonoBehaviour
{
    [SerializeField] private int _initialHealth = 3;
    [SerializeField] private int _value = 1;
    
    [SerializeField] private float _speedWhenCollected;
    
    private Collider _collider;
    private Outline _outline;
    private Renderer _renderer;
    
    public RessourceType RessourceType = RessourceType.Fer;
    
    public ushort Id;
    
    private int _currHealth;

    private bool _isInTravel;
    private bool _isInTravelBack;
    public bool IsCollected { private set; get; }

    private Vector3 _startPos;
    
    public PlayerIdentity PlayerCollector { private set; get; }

    private float _initialDistance;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _outline = GetComponentInChildren<Outline>();
        _renderer = GetComponentInChildren<Renderer>();
        
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
        _renderer.enabled = true;
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
        
        if(player.GetId == NetworkManager.Instance.LocalPlayer.GetId)
            RessourceManager.Instance.AddRessource(RessourceType, _value);

        _renderer.enabled = false;

        IsCollected = true;
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
