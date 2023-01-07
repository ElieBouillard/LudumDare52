using RiptideNetworking;
using UnityEngine;

public class ClientMessages : MonoBehaviour
{
    internal enum MessagesId : ushort
    {
        ClientConnected = 1,
        StartGame,
        Ready,
        Movements,
        Anims,
        Shoot,
    }
    
    #region Send
    public void SendClientConnected(ulong steamId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.ClientConnected);
        message.AddULong(steamId);
        NetworkManager.Instance.Client.Send(message);
    }
    
    public void SendStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.StartGame);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendReady()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.Ready);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendMovements(Vector3 pos, Quaternion rot)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.Movements);
        message.AddVector3(pos);
        message.AddQuaternion(rot);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendAnims(Vector2 velocity, bool jump)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.Anims);
        message.AddVector2(velocity);
        message.AddBool(jump);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendShoot(Vector3 pos, bool hit, Vector3 normal)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.Shoot);
        message.AddVector3(pos);
        message.AddBool(hit);
        message.AddVector3(normal);
        NetworkManager.Instance.Client.Send(message);
    }
    #endregion

    #region Received
    [MessageHandler((ushort) ServerMessages.MessagesId.PlayerConnectedToLobby)]
    private static void OnClientConnectedToLobby(Message message)
    {
        LobbyManager.Instance.AddPlayerToLobby(message.GetUShort(), message.GetULong());
    } 
    
    [MessageHandler((ushort) ServerMessages.MessagesId.PlayerDisconnected)]
    private static void OnClientDisconnected(Message message)
    {
        ushort id = message.GetUShort();
        
        switch (NetworkManager.Instance.GameState)
        {
            case GameState.Lobby:
                LobbyManager.Instance.RemovePlayerFromLobby(id);
                break;
            
            case GameState.Gameplay:
                GameManager.Instance.RemovePlayerFromGame(id);
                break;
        }
    } 
    
    [MessageHandler((ushort) ServerMessages.MessagesId.StartGame)]
    private static void OnServerStartGame(Message message)
    {
        NetworkManager.Instance.OnServerStartGame();
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.InitializeGameplay)]
    private static void OnServerInitializeClient(Message message)
    {
        GameManager.Instance.SpawnPlayers();
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.Movements)]
    private static void OnServerMovements(Message message)
    {
        ushort id = message.GetUShort();

        if (!NetworkManager.Instance.Players.ContainsKey(id)) return;

        if (NetworkManager.Instance.Players[id] is PlayerLobbyIdentity) return;
        
        ((PlayerGameIdentity) NetworkManager.Instance.Players[id]).MovementReceiver.SetState(message.GetVector3(), message.GetQuaternion());
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.Anims)]
    private static void OnServerAnims(Message message)
    {
        ushort id = message.GetUShort();

        if (!NetworkManager.Instance.Players.ContainsKey(id)) return;
        
        if (NetworkManager.Instance.Players[id] is PlayerLobbyIdentity) return;
        
        ((PlayerGameIdentity) NetworkManager.Instance.Players[id]).Animations.SetAnim(message.GetVector2(), message.GetBool());
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.Shoot)]
    private static void OnServerShoot(Message message)
    {
        ushort id = message.GetUShort();

        Vector3 pos = message.GetVector3();
        bool hit = message.GetBool();
        Vector3 normal = message.GetVector3();
        
        if (!NetworkManager.Instance.Players.ContainsKey(id)) return;
        
        if (NetworkManager.Instance.Players[id] is PlayerLobbyIdentity) return;

        ((PlayerGameIdentity) NetworkManager.Instance.Players[id]).Aim.Shoot(pos, hit ? normal : null);
    }
    #endregion
}
