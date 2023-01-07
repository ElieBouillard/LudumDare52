using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _localPlayerPrefab;
    [SerializeField] private GameObject _otherPlayerPrefab;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        NetworkManager.Instance.GetClientMessages.SendReady();
    }

    public void SpawnPlayers()
    {
        NetworkManager networkManager = NetworkManager.Instance;

        PlayerIdentity[] playersTemp = networkManager.GetPlayers.Values.ToArray();

        foreach (var player in playersTemp)
        {
            AddPlayerInGame(player.GetId, player.GetSteamId);
        }
    }
    
    public void AddPlayerInGame(ushort playerId, ulong steamId)
    {
        NetworkManager networkManager = NetworkManager.Instance;

        GameObject playerObject = playerId == networkManager.GetClient.Id ? _localPlayerPrefab : _otherPlayerPrefab;

        Transform spawnPoint = null;
        
        for (int i = 0; i < networkManager.GetPlayers.Values.ToArray().Length; i++)
        {
            if (networkManager.GetPlayers.Values.ToArray()[i].GetId == playerId)
            {
                spawnPoint = _spawnPoints[i];
                break;
            }
        }

        GameObject playerTemp = Instantiate(playerObject, spawnPoint.position ,spawnPoint.rotation);
        PlayerGameIdentity playerIdentityTemp = playerTemp.GetComponent<PlayerGameIdentity>();

        if(networkManager.GetUseSteam) playerIdentityTemp.Initialize(playerId, steamId);
        else playerIdentityTemp.Initialize(playerId, $"Player : {playerId}");

        if(playerId == networkManager.GetClient.Id) networkManager.SetLocalPlayer(playerIdentityTemp);

        networkManager.GetPlayers[playerId] = playerIdentityTemp;
    }

    public void RemovePlayerFromGame(ushort playerId)
    {
        NetworkManager networkManager = NetworkManager.Instance;
        foreach (var player in networkManager.GetPlayers)
        {
            if (player.Key == playerId)
            {
                Destroy(player.Value.gameObject);
                networkManager.GetPlayers.Remove(player.Key);
                return;
            }
        }
    }
    
    public void ClearPlayerInGame()
    {
        NetworkManager networkManager = NetworkManager.Instance;
        
        networkManager.SetLocalPlayer(null);
        
        foreach (var player in networkManager.GetPlayers)
        {
            Destroy(player.Value.gameObject);
        }
            
        networkManager.GetPlayers.Clear();
    }
}
