using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerGameIdentity playerIdentity))
        {
            if (!playerIdentity.IsLocalPlayer) return;
            
            playerIdentity.LocalHealth.Death();
        }
    }
}
