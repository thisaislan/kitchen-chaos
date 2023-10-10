using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameLobby : MonoBehaviour
{
    private const string KEY_RELAY_JOIN_CODE = "relay_join_code";

    public static KitchenGameLobby Instance { get; private set; }

    public event EventHandler OnCreateLobbStarted;
    public event EventHandler OnCreateLobbFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;

    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> Lobbies;
    }

    public Lobby JoinedLobby { get; private set; }

    private float heartbeatTimer = 0f;
    private float listeLobbiesTimer = 0f;
    private float heartbeatTimerMax = 15f;
    private float listeLobbiesTimerMax = 3f;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

        InitializeUnityAuthentication();
    }

    private void Update()
    {
        HandleHeartBeat();
        HandlePeriodicListLobbies();
    }

    public async void CreateLobby(string lobbyName, bool isPrivate) 
    {
        try
        {
            OnCreateLobbStarted?.Invoke(this, EventArgs.Empty);

            JoinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName,
                KitchenGameMultiplayer.MAX_NUMBER_OF_PLAYERS,
                new CreateLobbyOptions
                {
                    IsLocked = isPrivate
                });

            var allocation = await AllocateRelay();
            var relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions 
            {
                Data = new Dictionary<string, DataObject> 
                {
                    {
                        KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                    }
                }
            });
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetworkScene(Loader.SceneName.CharacterSelectScene);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
            OnCreateLobbFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);

            JoinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            var relyJoinCode = JoinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            var joinAllocation = await JoinRelay(relyJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);

            JoinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            JoinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            var relyJoinCode = JoinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            var joinAllocation = await JoinRelay(relyJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));


            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithId(string lobbyCode)
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);

            JoinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyCode);

            JoinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            var relyJoinCode = JoinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            var joinAllocation = await JoinRelay(relyJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));


            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void DeleteLobby()
    {
        try
        {
            if (JoinedLobby != null)
            {
                await LobbyService.Instance.DeleteLobbyAsync(JoinedLobby.Id);
            }
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            if (JoinedLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);

                JoinedLobby = null;
            }
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void KickPlayer(string playerId)
    {
        try
        {
            if (IsLobbyHost())
            {
                await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, playerId);
            }
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            var initializationOptions = new InitializationOptions();
            
            // Uncomment this line if you want run more than one build on the same computer
            // initializationOptions.SetProfile(UnityEngine.Random.Range(1,1000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async void ListLobbies()
    {
        try
        {
            var queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                Lobbies = queryResponse.Results
            });

        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
    }

    private void HandlePeriodicListLobbies()
    {
        if (JoinedLobby == null &&
            AuthenticationService.Instance.IsSignedIn &&
            SceneManager.GetActiveScene().name.Equals(Loader.SceneName.LoobyScene.ToString()))
        {
            listeLobbiesTimer -= Time.deltaTime;

            if (listeLobbiesTimer <= 0f)
            {
                listeLobbiesTimer = listeLobbiesTimerMax;

                ListLobbies();
            }
        }
    }

    private void HandleHeartBeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;

            if (heartbeatTimer <= 0f)
            {
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(JoinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost() =>
        JoinedLobby != null && JoinedLobby.HostId == AuthenticationService.Instance.PlayerId;

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            return await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_NUMBER_OF_PLAYERS - 1);
        }
        catch (RelayServiceException ex)
        {
            Debug.LogException(ex);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            return await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (RelayServiceException ex)
        {
            Debug.LogException(ex);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            return await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException ex)
        {
            Debug.LogException(ex);
            return default;
        }
    }
}
