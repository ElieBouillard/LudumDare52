using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementsSender : MonoBehaviour
{
    private NetworkManager _networkManager;
    
    private Vector3 _lastPos;
    private Quaternion _lastRot;
    
    private void Awake()
    {
        _networkManager = NetworkManager.Instance;
    }

    private void FixedUpdate()
    {
        if (_lastPos != transform.position || _lastRot != transform.GetChild(0).rotation)
        {
            _networkManager.ClientMessages.SendMovements(transform.position, transform.GetChild(0).rotation);

            _lastPos = transform.position;
            _lastRot = transform.GetChild(0).rotation;
        }
    }
}