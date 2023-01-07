using System;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : Singleton<LobbyManager>
{
    public LobbyPanel LobbyPanel;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _lobbyPlayerPrefab;

    public void AddPlayerToLobby(ushort playerId, ulong steamId)
    {
        NetworkManager networkManager = NetworkManager.Instance;
        
        GameObject playerInstance = Instantiate(_lobbyPlayerPrefab, _spawnPoints[networkManager.GetPlayers.Count].position, Quaternion.identity);
        PlayerLobbyIdentity playerLobbyIdentityInstance = playerInstance.GetComponent<PlayerLobbyIdentity>();
        
        if(!networkManager.GetUseSteam) playerLobbyIdentityInstance.Initialize(playerId, GetPlayerName(networkManager.GetClient.Id, playerId));
        else playerLobbyIdentityInstance.Initialize(playerId, steamId);
        
        if (playerId == networkManager.GetClient.Id) networkManager.SetLocalPlayer(playerLobbyIdentityInstance);  

        networkManager.GetPlayers.Add(playerId, playerLobbyIdentityInstance);
    }

    public void RemovePlayerFromLobby(ushort playerId)
    {
        NetworkManager networkManager = NetworkManager.Instance;

        foreach (var player in networkManager.GetPlayers)
        {
            if (player.Key == playerId)
            {
                Destroy(player.Value.gameObject);
                networkManager.GetPlayers.Remove(player.Key);
                ReorganizeLobbyPosition();
                return;
            }
        }
    }

    private void ReorganizeLobbyPosition()
    {
        NetworkManager networkManager = NetworkManager.Instance;
        int posIndex = 0;
        foreach (var player in networkManager.GetPlayers)
        {
            player.Value.gameObject.transform.position = _spawnPoints[posIndex].position;
            posIndex++;
        }
    }
    
    public void ClearLobby()
    {
        NetworkManager networkManager = NetworkManager.Instance;
        
        networkManager.SetLocalPlayer(null);
        
        foreach (var player in networkManager.GetPlayers)
        {
            Destroy(player.Value.gameObject);
        }
            
        networkManager.GetPlayers.Clear();
    }
    
    private string GetPlayerName(ushort clientId, ushort playerId)
    {
        if (playerId == 1) return $"Host : {playerId}";

        string statueName = playerId == clientId ? "Local" : "Client";
        
        return $"{statueName} : {playerId}";
    }
}