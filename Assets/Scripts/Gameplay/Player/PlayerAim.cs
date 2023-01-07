using System;
using CMF;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Transform _camera;

    private ushort _playerId;
    private AdvancedWalkerController _controller;
    
    private Ressource _lastRessource;

    private void Start()
    {
        _playerId = GetComponent<PlayerIdentity>().GetId;
        _controller = GetComponent<AdvancedWalkerController>();
    }

    private void Update()
    {
        if (!Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, Mathf.Infinity)) return;
        
        if (hit.collider.TryGetComponent(out Ressource ressource))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!_controller.IsSprinting)
                {
                    GiveDamage(ressource);
                }
            }
            
            if (_lastRessource != ressource)
            {
                ressource.EnableOutline(true);
                _lastRessource = ressource;
            }
        }
        else
        {
            if (_lastRessource != null)
            {
                _lastRessource.EnableOutline(false);
                _lastRessource = null;
            }
        }
    }

    private void GiveDamage(Ressource ressource)
    {
        ressource.TakeDamage(_playerId);
    }
}
