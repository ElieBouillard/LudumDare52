using System.Runtime.InteropServices.WindowsRuntime;
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
        RessourceTravel,
        Death,
        GiveDamage,
        DropRessources
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

    public void SendRessourceTravel(ushort ressourceId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.RessourceTravel);
        message.AddUShort(ressourceId);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendDeath()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.Death);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendDamage(ushort playerHit, float damage)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.GiveDamage);
        message.AddUShort(playerHit);
        message.AddFloat(damage);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendOnDropRessources(int ferAmount, int plasticAmount, int energyAmount)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.DropRessources);
        message.AddInt(ferAmount);
        message.AddInt(plasticAmount);
        message.AddInt(energyAmount);
        NetworkManager.Instance.Client.Send(message);
    }
    #endregion

    #region Received
    [MessageHandler((ushort) ServerMessages.MessagesId.PlayerConnectedToLobby)]
    private static void OnClientConnectedToLobby(Message message)
    {
        LobbyManager.Instance.AddPlayerToLobby(message.GetUShort(), message.GetULong(), message.GetInt());
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

    [MessageHandler((ushort) ServerMessages.MessagesId.InitializeGame)]
    private static void OnServerInitializeGame(Message message)
    {
        PanelManager.Instance.EnableGame();
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

    [MessageHandler((ushort) ServerMessages.MessagesId.RessourceTravel)]
    private static void OnServerRessourceTravel(Message message)
    {
        ushort playerId = message.GetUShort();
        ushort ressourceId = message.GetUShort();
        
        foreach (var ressource in RessourceManager.Instance.Ressources)
        {
            if (ressource.Id == ressourceId)
            {
                ressource.InitializeTravelToPlayer(playerId);
                return;
            }
        }
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.Death)]
    private static void OnServerDeath(Message message)
    {
        ushort id = message.GetUShort();
        
        ((PlayerGameIdentity)NetworkManager.Instance.Players[id]).DistantHealth.Death();
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.GiveDamage)]
    private static void OnServerGiveDamage(Message message)
    {
        ushort playerHitId = message.GetUShort();
        float damage = message.GetFloat();
        
        if (playerHitId == NetworkManager.Instance.LocalPlayer.GetId)
        {
            ((PlayerGameIdentity)NetworkManager.Instance.Players[playerHitId]).LocalHealth.TakeDamage(damage);
        }
        else
        {
            ((PlayerGameIdentity)NetworkManager.Instance.Players[playerHitId]).DistantHealth.TakeDamage(damage);
        }
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.DropRessources)]
    private static void OnServerDropRessources(Message message)
    {
        ushort id = message.GetUShort();
        
        RessourceManager.Instance.AddRessourceToBase(id,message.GetInt(),message.GetInt(),message.GetInt());
    }
    #endregion
}
