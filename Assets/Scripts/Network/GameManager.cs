using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Transform[] _spawnPointsTeam0;
    [SerializeField] private Transform[] _spawnPointsTeam1;
    
    [SerializeField] private GameObject _localPlayerPrefab;
    [SerializeField] private GameObject _distantPlayerPrefab;


    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _defeatPanel;

    [SerializeField] private GameObject _clipTeam0;
    [SerializeField] private GameObject _clipTeam1;
    
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
        NetworkManager.Instance.ClientMessages.SendReady();
    }

    public Vector3 GetRespawnPos(int teamId)
    {
        return teamId == 0 ? _spawnPointsTeam0[0].position : _spawnPointsTeam1[0].position;
    }

    public void SetEndGame(bool _isVictory)
    {
        _victoryPanel.SetActive(_isVictory);
        _defeatPanel.SetActive(!_isVictory);

        if (_isVictory && NetworkManager.Instance.LocalPlayer.TeamId == 0)
        {
            _clipTeam0.SetActive(true);
        }
        
        if (_isVictory && NetworkManager.Instance.LocalPlayer.TeamId == 1)
        {
            _clipTeam1.SetActive(true);
        }
        
        if (!_isVictory && NetworkManager.Instance.LocalPlayer.TeamId == 0)
        {
            _clipTeam1.SetActive(true);
        }
        
        if (!_isVictory && NetworkManager.Instance.LocalPlayer.TeamId == 0)
        {
            _clipTeam0.SetActive(true);
        }
        
        ((PlayerGameIdentity)NetworkManager.Instance.LocalPlayer).EnableInput(false);
        
        PanelManager.Instance.EnableCursor(true);
    }
    
    public void SpawnPlayers()
    {
        NetworkManager networkManager = NetworkManager.Instance;

        PlayerIdentity[] playersTemp = networkManager.Players.Values.ToArray();

        foreach (var player in playersTemp)
        {
            AddPlayerInGame(player.GetId, player.GetSteamId, player.TeamId);
        }
    }
    
    public void AddPlayerInGame(ushort playerId, ulong steamId, int teamId)
    {
        NetworkManager networkManager = NetworkManager.Instance;

        GameObject playerObject = playerId == networkManager.Client.Id ? _localPlayerPrefab : _distantPlayerPrefab;

        Transform spawnPoint = null;

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

        GameObject playerTemp = Instantiate(playerObject, spawnPoint.position ,spawnPoint.rotation);
        PlayerGameIdentity playerIdentityTemp = playerTemp.GetComponent<PlayerGameIdentity>();

        if(networkManager.UseSteam) playerIdentityTemp.Initialize(playerId, steamId, teamId);
        else playerIdentityTemp.Initialize(playerId, $"Player : {playerId}", teamId);

        if(playerId == networkManager.Client.Id) networkManager.SetLocalPlayer(playerIdentityTemp);

        networkManager.Players[playerId] = playerIdentityTemp;
        
        if(teamId == 0) networkManager.Team0[playerId] = playerIdentityTemp;
        else if(teamId == 1) networkManager.Team1[playerId] = playerIdentityTemp;
    }

    public void RemovePlayerFromGame(ushort playerId)
    {
        NetworkManager networkManager = NetworkManager.Instance;
        
        Destroy(networkManager.Players[playerId].gameObject);
        networkManager.Players.Remove(playerId);
    }
    
    public void ClearPlayerInGame()
    {
        NetworkManager networkManager = NetworkManager.Instance;
        
        networkManager.SetLocalPlayer(null);
        
        foreach (var player in networkManager.Players)
        {
            Destroy(player.Value.gameObject);
        }
            
        networkManager.Players.Clear();
    }
}