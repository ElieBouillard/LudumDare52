using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour
{
    [SerializeField] private int _initialHealth = 3;
    [SerializeField] private int _value = 1;
    
    [SerializeField] private float _speedWhenCollected;
    
    private Collider _collider;
    private Outline _outline;

    public RessourceType RessourceType = RessourceType.Fer;
    
    public ushort Id;
    
    private int _currHealth;

    private bool _isInTravel;

    private Transform _targetPlayer;

    private float _initialDistance;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _outline = GetComponentInChildren<Outline>();
        
        _currHealth = _initialHealth;
        
        _outline.OutlineWidth = 0f;
    }

    private void Update()
    {
        if (!_isInTravel) return;

        transform.position += (_targetPlayer.transform.position + Vector3.up - transform.position).normalized * _speedWhenCollected * Time.deltaTime;
        transform.Rotate(new Vector3(180f, 180f, 0) * Time.deltaTime);

        float currDistance = (_targetPlayer.transform.position + Vector3.up - transform.position).magnitude;
        
        transform.localScale = Vector3.one * (currDistance  / _initialDistance);

        if (currDistance <= 0.5f)
        {
            Collected();
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

        _targetPlayer = NetworkManager.Instance.Players[playerId].transform;

        _initialDistance = (_targetPlayer.transform.position - transform.position).magnitude;

        _collider.enabled = false;
        
        _isInTravel = true;
    }
    
    public void Collected()
    {
        PlayerIdentity player = _targetPlayer.GetComponent<PlayerIdentity>();
        
        Debug.Log($"Collected by player {player.GetId}");

        if(player.TeamId == NetworkManager.Instance.LocalPlayer.TeamId)
            RessourceManager.Instance.AddRessource(RessourceType, _value);
        
        Destroy(gameObject);
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
