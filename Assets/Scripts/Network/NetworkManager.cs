using System;
using System.Collections.Generic;
using RiptideNetworking;
using RiptideNetworking.Transports.SteamTransport;
using RiptideNetworking.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using SteamClient = RiptideNetworking.Transports.SteamTransport.SteamClient;

public class NetworkManager : Singleton<NetworkManager>
{
    #region UnityInspector
    [Header("NetworkSettings")] [Space(10)]
    [SerializeField] private bool _useSteam;
    [SerializeField] private ushort _port = 7777;
    [SerializeField] private ushort _maxPlayer = 4;
    #endregion
    
    #region Fields
    private Server _server;
    private Client _client; 
    /*Set To Private */ public GameState _gameState = GameState.OffLine;
    private Dictionary<ushort, PlayerIdentity> _players = new Dictionary<ushort, PlayerIdentity>();
    private PlayerIdentity  _localPlayer;
    private ClientMessages _clientMessages;
    private ServerMessages _serverMessages;
    #endregion

    #region Getters
    public bool GetUseSteam => _useSteam;
    public Server GetServer => _server;
    public Client GetClient => _client;
    public GameState GetGameState => _gameState;
    public Dictionary<ushort, PlayerIdentity> GetPlayers => _players;
    public PlayerIdentity GetLocalPlayer => _localPlayer;
    public ClientMessages GetClientMessages => _clientMessages;
    public ServerMessages GetServerMessages => _serverMessages;
    public ushort GetMaxPlayer => _maxPlayer;
    #endregion

    #region Setters
    public void SetLocalPlayer(PlayerIdentity player) => _localPlayer = player;
    #endregion
    
    protected override void Awake()
    {
        base.Awake();

        _clientMessages = gameObject.AddComponent<ClientMessages>();
        _serverMessages = gameObject.AddComponent<ServerMessages>();

        if (_useSteam)
        {
            gameObject.AddComponent<SteamManager>();
            gameObject.AddComponent<SteamLobbyManager>();
        }
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        SteamServer steamServer = new SteamServer();
        InitializeClient(steamServer);
        InitializeServer(steamServer);
    }

    private void FixedUpdate()
    {
        _client.Tick();
        
        if(_server.IsRunning) _server.Tick();
    }

    private void OnApplicationQuit()
    {
        switch (_gameState)
        {
            case GameState.OffLine:
                break;
            case GameState.Lobby:
                Leave();
                break;
            case GameState.Gameplay:
                Leave();
                break;
        }
    }

    private void InitializeClient(SteamServer steamServer)
    {
        _client = _useSteam ? new Client(new SteamClient(steamServer)) : new Client();

        _client.Connected += ClientOnConnected;
        _client.Disconnected += ClientOnDisconnected;
        _client.ConnectionFailed += ClientOnConnectionFailed;
        _client.ClientConnected += ClientOnPlayerJoin;
        _client.ClientDisconnected += ClientOnPlayerLeft;
    }

    private void InitializeServer(SteamServer steamServer)
    {
        _server = _useSteam ? new Server(steamServer) : new Server();

        _server.ClientConnected += ServerOnClientConnected;
        _server.ClientDisconnected += ServerOnClientDisconnected;
    }
    
    #region ClientCallbacks
    private void ClientOnConnected(object sender, EventArgs e)
    {
        _gameState = GameState.Lobby;

        _clientMessages.SendClientConnected(_useSteam ? (ulong)SteamUser.GetSteamID() : new ulong());

        PanelManager.Instance.EnablePanel(PanelType.Lobby);
    }

    private void ClientOnDisconnected(object sender, EventArgs e)
    {
        switch (_gameState)
        {
            case GameState.Lobby:
                LobbyManager.Instance.ClearLobby();
                PanelManager.Instance.EnablePanel(PanelType.MainMenu);
                break;
            case GameState.Gameplay:
                GameManager.Instance.ClearPlayerInGame();
                SceneManager.LoadScene("StartMenuScene", LoadSceneMode.Single);
                break;
        }
        
        _gameState = GameState.OffLine;
        
        if(!_useSteam) return;
        SteamLobbyManager.Instance.LeaveLobby();
    }
    
    private void ClientOnConnectionFailed(object sender, EventArgs e)
    {
        
    }
    
    private void ClientOnPlayerJoin(object sender, ClientConnectedEventArgs e)
    {
        
    }
    
    private void ClientOnPlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        
    }
    #endregion

    #region ServerCallbacks
    private void ServerOnClientConnected(object sender, ServerClientConnectedEventArgs e)
    {
        switch (_gameState)
        {
            case GameState.Gameplay:
                _server.DisconnectClient(e.Client.Id);
                break;
        }
    }
    
    private void ServerOnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        _serverMessages.SendPlayerDisconnected(e.Id);
    }
    #endregion

    #region Client
    public void StartHost()
    {
        if (_useSteam)
        {
            SteamLobbyManager.Instance.CreateLobby(); ;
        }
        else
        {
            _server.Start(_port, _maxPlayer);
            _client.Connect($"127.0.0.1:{_port}");
        }
    }
    
    public void JoinLobby()
    {
        if (_useSteam) return;
        _client.Connect($"127.0.0.1:{_port}");
    }
    
    public void Leave()
    {
        _client.Disconnect();
        ClientOnDisconnected(new object(), EventArgs.Empty);
        _server.Stop();
    }

    public void StartGame()
    {
        if (_localPlayer.GetId != 1) return;
        
        _clientMessages.SendStartGame();
    }

    public void OnServerStartGame()
    {
        _gameState = GameState.Gameplay;
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
    #endregion

    #region Server
    
    #endregion
}

public enum GameState
{
    OffLine = 1,
    Lobby,
    Gameplay,
}