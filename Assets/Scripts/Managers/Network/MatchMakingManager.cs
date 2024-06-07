using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;


namespace Managers.Network
{

 public class MatchMakingManager : SingletonManager<MatchMakingManager>
 {
  #region Fields
  
  private Lobby _connectedLobby;
  private QueryResponse _lobbies;
  private UnityTransport _transport;
  private const string JoinCodeKey = "j";
  private string _playerId;
  
  #endregion

  #region Properties

  #endregion

  #region Unity Methods
  void Awake()
  {
   _transport = FindObjectOfType<UnityTransport>();
  }
  
  #endregion

  #region Private Methods
  
public async void CreateOrJoinLobby() {
    await Authenticate();
        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();

        if (_connectedLobby != null)   UIManager.Instance.ControlMainScreen(false);;
    }

    private async Task Authenticate() {
        var options = new InitializationOptions();

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _playerId = AuthenticationService.Instance.PlayerId;
    }

    private async Task<Lobby> QuickJoinLobby() {
        try {
            // Attempt to join a lobby in progress
            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            // If we found one, grab the relay allocation details
            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);

            // Set the details to the transform
            SetTransformAsClient(a);

            // Join the game room as a client
            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch {
            Debug.Log($"No lobbies available via quick join");
            return null;
        }
    }

    private async Task<Lobby> CreateLobby() {
        try {
            const int maxPlayers = 2;

            // Create a relay allocation and generate a join code to share with the lobby
            var a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            // Create a lobby, adding the relay join code to the lobby data
            var options = new CreateLobbyOptions {
                Data = new Dictionary<string, DataObject> { { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };
            var lobby = await Lobbies.Instance.CreateLobbyAsync("Useless Lobby Name", maxPlayers, options);

            // Send a heartbeat every 15 seconds to keep the room alive
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            // Set the game room to use the relay allocation
            _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

            // Start the room. I'm doing this immediately, but maybe you want to wait for the lobby to fill up
            NetworkManager.Singleton.StartHost();
            return lobby;
        }
        catch {
            Debug.LogFormat("Failed creating a lobby");
            return null;
        }
    }

    private void SetTransformAsClient(JoinAllocation a) {
        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
    }

    private static IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds) {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true) {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private async void OnDestroy()
    {
        try
        {
            StopAllCoroutines();

            if (_connectedLobby != null)
            {
                if (_connectedLobby.HostId == _playerId)
                {
                    await Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                }
                else
                {
                    await Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error shutting down lobby: {e}");
        }
    }

    public async void ReturnToHome()
    {
        UIManager.Instance.ControlMainScreen(true);

        try
        {
            if (_connectedLobby != null)
            {
                if (_connectedLobby.HostId == _playerId)
                {
                    await Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                }
                else
                {
                    await Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error shutting down lobby: {e}");
        }
        finally
        {
            _connectedLobby = null;
            AuthenticationService.Instance.SignOut();
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }
  #endregion

 }
}
