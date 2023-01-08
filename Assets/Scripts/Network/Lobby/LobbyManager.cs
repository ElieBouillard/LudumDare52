using System;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : Singleton<LobbyManager>
{
    public LobbyPanel LobbyPanel;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _lobbyPlayerPrefab;

    public void AddPlayerToLobby(ushort playerId, ulong steamId, int teamId)
    {
        NetworkManager networkManager = NetworkManager.Instance;
        
        GameObject playerInstance = Instantiate(_lobbyPlayerPrefab, _spawnPoints[networkManager.Players.Count].position, Quaternion.identity);
        PlayerLobbyIdentity playerLobbyIdentityInstance = playerInstance.GetComponent<PlayerLobbyIdentity>();
        
        if(!networkManager.UseSteam) playerLobbyIdentityInstance.Initialize(playerId, GetPlayerName(networkManager.Client.Id, playerId), teamId);
        else playerLobbyIdentityInstance.Initialize(playerId, steamId, teamId);
        
        if (playerId == networkManager.Client.Id) networkManager.SetLocalPlayer(playerLobbyIdentityInstance);  

        networkManager.Players.Add(playerId, playerLobbyIdentityInstance);
        
        if(teamId == 0) networkManager.Team0.Add(playerId, playerLobbyIdentityInstance);
        else if(teamId == 1) networkManager.Team1.Add(playerId, playerLobbyIdentityInstance);
    }

    public void RemovePlayerFromLobby(ushort playerId)
    {
        NetworkManager networkManager = NetworkManager.Instance;

        Destroy(networkManager.Players[playerId].gameObject);
        networkManager.Players.Remove(playerId);
        ReorganizeLobbyPosition();
    }

    private void ReorganizeLobbyPosition()
    {
        NetworkManager networkManager = NetworkManager.Instance;
        int posIndex = 0;
        foreach (var player in networkManager.Players)
        {
            player.Value.gameObject.transform.position = _spawnPoints[posIndex].position;
            posIndex++;
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