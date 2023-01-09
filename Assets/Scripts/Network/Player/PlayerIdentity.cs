using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using Steamworks;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerIdentity : MonoBehaviour
{
    #region Fields
    private ushort _id;
    protected ulong _steamId;
    private bool _isLocalPlayer;
    public int TeamId = -1;
    #endregion
    
    #region Getters
    public ushort GetId => _id;
    public ulong GetSteamId => _steamId;
    public bool IsLocalPlayer => _isLocalPlayer;
    #endregion

    public Material[] TeamMats;
    public Renderer Renderer;
    
    public virtual void Initialize(ushort id, string newName, int teamId)
    {
        _id = id;

        if (_id == NetworkManager.Instance.Client.Id) { _isLocalPlayer = true; }

        gameObject.name = newName;

        TeamId = teamId;

        Renderer.material = TeamMats[teamId];
    }
    
    public virtual void Initialize(ushort id, ulong steamId, int teamId)
    {
        Initialize(id, SteamFriends.GetFriendPersonaName((CSteamID)steamId), teamId);
        
        _steamId = steamId;
    }
}