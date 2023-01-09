using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : Singleton<LobbyManager>
{
    public LobbyPanel LobbyPanel;
    [SerializeField] private Transform[] _spawnPointsTeam0;
    [SerializeField] private Transform[] _spawnPointsTeam1;
    [SerializeField] private Transform _spawnPointNoTeam;
    
    [SerializeField] private GameObject _lobbyPlayerPrefab;
    public PlayerIdentity NoTeamPlayer;

    public void AddPlayerToLobby(ushort playerId, ulong steamId, int teamId)
    {
        NetworkManager networkManager = NetworkManager.Instance;

        Transform spawnPoint = null;

        GameObject playerInstance = Instantiate(_lobbyPlayerPrefab, Vector3.zero, Quaternion.identity);
        PlayerLobbyIdentity playerLobbyIdentityInstance = playerInstance.GetComponent<PlayerLobbyIdentity>();
        
        if(!networkManager.UseSteam) playerLobbyIdentityInstance.Initialize(playerId, GetPlayerName(networkManager.Client.Id, playerId), teamId);
        else playerLobbyIdentityInstance.Initialize(playerId, steamId, teamId);
        
        if (playerId == networkManager.Client.Id) networkManager.SetLocalPlayer(playerLobbyIdentityInstance);  

        networkManager.Players.Add(playerId, playerLobbyIdentityInstance);
        
        if(teamId == 0) networkManager.Team0.Add(playerId, playerLobbyIdentityInstance);
        else if(teamId == 1) networkManager.Team1.Add(playerId, playerLobbyIdentityInstance);
        
        if (teamId == 0)
        {
            for (int i = 0; i < networkManager.Team0.Count; i++)
            {
                if (networkManager.Team0.Values.ToArray()[i].GetId == playerId)
                {
                    spawnPoint = _spawnPointsTeam0[i];
                    break;
                }
            }
        }
        else if (teamId == 1)
        {
            for (int i = 0; i < networkManager.Team1.Count; i++)
            {
                if (networkManager.Team1.Values.ToArray()[i].GetId == playerId)
                {
                    spawnPoint = _spawnPointsTeam1[i];
                    break;
                }
            }
        }

        playerInstance.transform.position = spawnPoint.position;
        playerInstance.transform.rotation = spawnPoint.rotation;
    }

    public void SwapToTeam0()
    {
        if(NetworkManager.Instance.LocalPlayer.TeamId == 0) return;
        if(NetworkManager.Instance.Team0.Count == 2) return;
        
        NetworkManager.Instance.ClientMessages.SendSwapTeam(0);
    }

    public void SwapToTeam1()
    {
        if(NetworkManager.Instance.LocalPlayer.TeamId == 1) return;
        if(NetworkManager.Instance.Team1.Count == 2) return;
        NetworkManager.Instance.ClientMessages.SendSwapTeam(1);
    }

    public void SwapToNoTeam()
    {
        if(NetworkManager.Instance.LocalPlayer.TeamId == -1) return;
        if (NoTeamPlayer != null) return;
        NetworkManager.Instance.ClientMessages.SendSwapTeam(-1);
    }

    public void ServerSwapTeam(ushort playerId, int teamId)
    {
        if (teamId != -1)
        {
            if (NoTeamPlayer != null)
            {
                if (NoTeamPlayer.GetId == playerId)
                    NoTeamPlayer = null;
            }
        }
        
        if (teamId == 0)
        {
            if (NetworkManager.Instance.Team1.ContainsKey(playerId))
                NetworkManager.Instance.Team1.Remove(playerId);
            
            NetworkManager.Instance.Team0.Add(playerId, NetworkManager.Instance.Players[playerId]);
            NetworkManager.Instance.Players[playerId].TeamId = 0;
        }
        else if (teamId == 1)
        {
            if (NetworkManager.Instance.Team0.ContainsKey(playerId))
                NetworkManager.Instance.Team0.Remove(playerId);
            
            NetworkManager.Instance.Team1.Add(playerId, NetworkManager.Instance.Players[playerId]);
            NetworkManager.Instance.Players[playerId].TeamId = 1;
        }
        else
        {
            if (NetworkManager.Instance.Team1.ContainsKey(playerId))
                NetworkManager.Instance.Team1.Remove(playerId);
            if (NetworkManager.Instance.Team0.ContainsKey(playerId))
                NetworkManager.Instance.Team0.Remove(playerId);

            NoTeamPlayer = NetworkManager.Instance.Players[playerId];
            NetworkManager.Instance.Players[playerId].TeamId = -1;
        }

        NetworkManager.Instance.Players[playerId].ChangeColor(teamId);
        
        ReorganizeLobbyPosition();
    }
    
    public void RemovePlayerFromLobby(ushort playerId)
    {
        NetworkManager networkManager = NetworkManager.Instance;

        Destroy(networkManager.Players[playerId].gameObject);
        networkManager.Players.Remove(playerId);

        if (NetworkManager.Instance.Team0.ContainsKey(playerId))
            NetworkManager.Instance.Team0.Remove(playerId);
        if (NetworkManager.Instance.Team1.ContainsKey(playerId))
            NetworkManager.Instance.Team1.Remove(playerId);
    }

    private void ReorganizeLobbyPosition()
    {
        NetworkManager networkManager = NetworkManager.Instance;

        if (NoTeamPlayer != null)
            NoTeamPlayer.transform.position = _spawnPointNoTeam.position;

        for (int i = 0; i < networkManager.Team0.Values.ToArray().Length; i++)
        {
            networkManager.Team0.Values.ToArray()[i].transform.position = _spawnPointsTeam0[i].position;
        }
        
        for (int i = 0; i < networkManager.Team1.Values.ToArray().Length; i++)
        {
            networkManager.Team1.Values.ToArray()[i].transform.position = _spawnPointsTeam1[i].position;
        }
    }
    
    public void ClearLobby()
    {
        NetworkManager networkManager = NetworkManager.Instance;
        
        networkManager.SetLocalPlayer(null);
        
        foreach (var player in networkManager.Players)
        {
            Destroy(player.Value.gameObject);
        }
            
        networkManager.Players.Clear();
    }
    
    private string GetPlayerName(ushort clientId, ushort playerId)
    {
        if (playerId == 1) return $"Host : {playerId}";

        string statueName = playerId == clientId ? "Local" : "Client";
        
        return $"{statueName} : {playerId}";
    }
}