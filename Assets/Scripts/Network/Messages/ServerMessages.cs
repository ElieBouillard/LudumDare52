using RiptideNetworking;
using UnityEngine;

public class ServerMessages : MonoBehaviour
{
    internal enum MessagesId : ushort
    {
        PlayerConnectedToLobby = 1,
        PlayerDisconnected,
        StartGame,
        InitializeGameplay,
        InitializeGame,
        Movements,
        Anims,
        Shoot,
        RessourceTravel,
        Death,
        GiveDamage,
    }

    #region Send
    public static void SendPlayerConnectedToLobby(ushort newPlayerId, ulong steamId)
    {
        int teamId = NetworkManager.Instance.Players.Count % 2 == 0 ? 0 : 1;

        foreach (var player in NetworkManager.Instance.Players)
        {
            Message message1 = Message.Create(MessageSendMode.reliable, MessagesId.PlayerConnectedToLobby);
            message1.AddUShort(player.Value.GetId);
            message1.AddULong(player.Value.GetSteamId);
            message1.AddInt(player.Value.TeamId);
            NetworkManager.Instance.Server.Send(message1, newPlayerId);
        }
        
        Message message2 = Message.Create(MessageSendMode.reliable, MessagesId.PlayerConnectedToLobby);
        message2.AddUShort(newPlayerId);
        message2.AddULong(steamId);
        message2.AddInt(teamId);   
        NetworkManager.Instance.Server.SendToAll(message2);
    }
    
    public void SendPlayerDisconnected(ushort playerId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.PlayerDisconnected);
        message.AddUShort(playerId);
        NetworkManager.Instance.Server.SendToAll(message, playerId);
    }

    private static void SendHostStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.StartGame);
        NetworkManager.Instance.Server.SendToAll(message);
    }

    private static void SendInitializeClient(ushort id)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.InitializeGameplay);
        NetworkManager.Instance.Server.Send(message, id);
    }

    private static void SendInitializeGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.InitializeGame);
        NetworkManager.Instance.Server.SendToAll(message);
    }
    
    private static void SendMovements(ushort id, Vector3 pos, Quaternion rot)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.Movements);
        message.AddUShort(id);
        message.AddVector3(pos);
        message.AddQuaternion(rot);
        NetworkManager.Instance.Server.SendToAll(message, id);
    }

    private static void SendAnims(ushort id, Vector2 velocity, bool jump)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.Anims);
        message.AddUShort(id);
        message.AddVector2(velocity);
        message.AddBool(jump);
        NetworkManager.Instance.Server.SendToAll(message,id);
    }

    private static void SendShoot(ushort id, Vector3 pos, bool hit, Vector3 normal)
    {
        Message message =Message.Create(MessageSendMode.reliable, MessagesId.Shoot);
        message.AddUShort(id);
        message.AddVector3(pos);
        message.AddBool(hit);
        message.AddVector3(normal);
        NetworkManager.Instance.Server.SendToAll(message, id);
    }

    private static void SendRessourceTravel(ushort playerId, ushort ressourceId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.RessourceTravel);
        message.AddUShort(playerId);
        message.AddUShort(ressourceId);
        NetworkManager.Instance.Server.SendToAll(message, playerId);
    }

    private static void SendDeath(ushort id)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.Death);
        message.AddUShort(id);
        NetworkManager.Instance.Server.SendToAll(message, id);
    }

    private static void SendGiveDamage(ushort playerShoot, ushort playerHit, float damage)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.GiveDamage);
        message.AddUShort(playerHit);
        message.AddFloat(damage);
        NetworkManager.Instance.Server.SendToAll(message, playerShoot);
    }
    #endregion

    #region Received
    [MessageHandler((ushort) ClientMessages.MessagesId.ClientConnected)]
    private static void OnClientConnected(ushort id, Message message)
    {
        SendPlayerConnectedToLobby(id, message.GetULong());
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.StartGame)]
    private static void OnClientStartGame(ushort id, Message message)
    {
        if(id != 1) return;
        SendHostStartGame();
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.Ready)]
    private static void OnClientReady(ushort id, Message message)
    {
        SendInitializeClient(id);

        NetworkManager.Instance.PlayerReadyCount++;
        
        if (NetworkManager.Instance.Players.Count == NetworkManager.Instance.PlayerReadyCount)
        {
            SendInitializeGame();
        }
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.Movements)]
    private static void OnClientMovements(ushort id, Message message)
    {
        SendMovements(id, message.GetVector3(), message.GetQuaternion());
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.Anims)]
    private static void OnClientAnims(ushort id, Message message)
    {
        SendAnims(id, message.GetVector2(), message.GetBool()); 
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.Shoot)]
    private static void OnClientShoot(ushort id, Message message)
    {
        SendShoot(id, message.GetVector3(), message.GetBool(), message.GetVector3());
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.RessourceTravel)]
    private static void OnRessourceTravel(ushort id, Message message)
    {
        SendRessourceTravel(id, message.GetUShort());
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.Death)]
    private static void OnDeath(ushort id, Message message)
    {
        SendDeath(id);
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.GiveDamage)]
    private static void OnGiveDamage(ushort id, Message message)
    {
        SendGiveDamage(id, message.GetUShort(), message.GetFloat());
    }
    #endregion
}