using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class KitchenGameMultiplayer : NetworkBehaviour
{

    public const int MAX_NUMBER_OF_PLAYERS = 4;
    public const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "player_name_multiplayer";

    public static KitchenGameMultiplayer Instance { get; private set; }

    public static bool PlayMultplayer = true;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDatasNetworkListChangedEvent;

    [SerializeField] private KichenObjectList kichenObjectList;
    [SerializeField] private List<UnityEngine.Color> colorList;

    public string PlayerName 
    {
        get 
        {
            return playerName; 
        }
        set 
        {
            playerName = value;
            PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
        } 
    }

    private string playerName;

    private NetworkList<PlayerData> playerDatasNetworkList;

    private void Awake()
    {
        Instance = this;
        playerDatasNetworkList = new NetworkList<PlayerData>();
        playerDatasNetworkList.OnListChanged += OnPlayerDatasNetworkListChanged;

        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "Player" + UnityEngine.Random.Range(100, 1000));

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!PlayMultplayer)
        {
            KitchenGameLobby.Instance.CreateLobby("Lobby Name", true);
        }
    }

    public override void OnDestroy()
    {
        playerDatasNetworkList.OnListChanged -= OnPlayerDatasNetworkListChanged;
        base.OnDestroy();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectScriptableObjectIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference
       )
    {
        if (kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject))
        {
            var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

            if (!kitchenObjectParent.HasKitchenObject())
            {
                var kitchenObjectScriptableObject = GetKitchenScriptableObjectFromIndex(kitchenObjectScriptableObjectIndex);
                var kitchenObject = Instantiate(kitchenObjectScriptableObject.prefab).GetComponent<KitchenObject>();
                var kitchenObjectNetworkObject = kitchenObject.GetComponent<NetworkObject>();

                kitchenObjectNetworkObject.Spawn(true);
                kitchenObject.SetKitchenObjecParent(kitchenObjectParent);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        if (kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject))
        {
            if (kitchenObjectNetworkObject != null)
            {
                ClearKicthenObjectOnParantClientRpc(kitchenObjectNetworkObjectReference);
                kitchenObjectNetworkObject.GetComponent<KitchenObject>().DestroySelf();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (IsColorAvailable(colorId))
        {
            var playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            var playerData = playerDatasNetworkList[playerDataIndex];
            playerData.colorId = colorId;
            playerDatasNetworkList[playerDataIndex] = playerData;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        var playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        var playerData = playerDatasNetworkList[playerDataIndex];
        playerData.playerName = playerName;
        playerDatasNetworkList[playerDataIndex] = playerData;

    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        var playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        var playerData = playerDatasNetworkList[playerDataIndex];
        playerData.playerId = playerId;
        playerDatasNetworkList[playerDataIndex] = playerData;
    }

    [ClientRpc]
    public void ClearKicthenObjectOnParantClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) 
    {
        if (kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject))
        {
            kitchenObjectNetworkObject.GetComponent<KitchenObject>().ClearKicthenObjectOnParant();
        }
    }

    public void SpawnKitchenObject(
       KitchenObjectScriptableObject kitchenObjectScriptableObject,
       IKitchenObjectParent kitchenObjectParent
       ) =>
        SpawnKitchenObjectServerRpc(GetKitchenScriptableObjectIndex(kitchenObjectScriptableObject), kitchenObjectParent.GetNetworkObject());

    public void DestroyKitchenObject(KitchenObject kitchenObject) =>
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);

    public int GetKitchenScriptableObjectIndex(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        kichenObjectList.kitchenObjectScriptableObjectsList.IndexOf(kitchenObjectScriptableObject);

    public KitchenObjectScriptableObject GetKitchenScriptableObjectFromIndex(int kitchenObjectScriptableObjectIndex) =>
        kichenObjectList.kitchenObjectScriptableObjectsList[kitchenObjectScriptableObjectIndex];

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += OnNetworkManagerServerConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkManagerServerConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkManagerServerDisconnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkManagerClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkManagerServerClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void OnNetworkManagerServerClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(PlayerName);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    } 

    public bool IsPlayerIndexConnected(int playerIndex) =>
        playerIndex < playerDatasNetworkList.Count;

    public PlayerData GetPlayerDataFormPlayerIndex(int playerIndex) =>
        playerDatasNetworkList[playerIndex];

    public PlayerData GetPlayerDataFormClientId(ulong clientId)
    {
        foreach (var playerData in playerDatasNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }

        }
        return default;
    }

    public PlayerData GetPlayerData() =>
        GetPlayerDataFormClientId(NetworkManager.Singleton.LocalClientId);

    public UnityEngine.Color GetPlayerColor(int colorId) =>
        colorList[colorId];

    public void ChangePlayerColor(int colorId) =>
        ChangePlayerColorServerRpc(colorId);

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (var i = 0; i < playerDatasNetworkList.Count; i++) 
        { 
            if (playerDatasNetworkList[i].clientId == clientId)
            {
                return i;
            }

        }
        return -1;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        OnNetworkManagerServerDisconnectedCallback(clientId);
    }

    private void OnPlayerDatasNetworkListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDatasNetworkListChangedEvent?.Invoke(this, EventArgs.Empty);
    }
    private bool IsColorAvailable(int colorId)
    {
        foreach (var playerData in playerDatasNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                return false;
            }
        }
        return true;
    }

    private int GetFirtsUnusedColorId()
    {
        for (var i = 0;i < colorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }

        return -1;
    }

    private void OnNetworkManagerServerConnectedCallback(ulong clientId)
    {
        playerDatasNetworkList.Add(new PlayerData { 
            clientId = clientId,
            colorId = GetFirtsUnusedColorId()
        });

        SetPlayerNameServerRpc(PlayerName);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void OnNetworkManagerServerDisconnectedCallback(ulong clientId)
    {
        for(var i = 0; i < playerDatasNetworkList.Count; i++)
        {
            var playerData = playerDatasNetworkList[i];

            if (playerData.clientId == clientId)
            {
                playerDatasNetworkList.RemoveAt(i);
            }
        }
    }

    private void OnNetworkManagerClientDisconnectCallback(ulong obj) =>
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);

    private void OnNetworkManagerServerConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.SceneName.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "The game has already started!";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_NUMBER_OF_PLAYERS)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "The game is full!";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }
}
