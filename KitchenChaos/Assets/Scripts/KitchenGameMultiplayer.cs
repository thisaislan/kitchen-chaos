using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    private const int MAX_NUMBER_OF_PLAYERS = 4;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDatasNetworkListChangedEvent;

    [SerializeField] private KichenObjectList kichenObjectList;
    [SerializeField] private List<Color> colorList;

    private NetworkList<PlayerData> playerDatasNetworkList;

    private void Awake()
    {
        Instance = this;
        playerDatasNetworkList = new NetworkList<PlayerData>();
        playerDatasNetworkList.OnListChanged += OnPlayerDatasNetworkListChanged;

        DontDestroyOnLoad(gameObject);
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
        var kitchenObjectScriptableObject = GetKitchenScriptableObjectFromIndex(kitchenObjectScriptableObjectIndex);
        var kitchenObject = Instantiate(kitchenObjectScriptableObject.prefab).GetComponent<KitchenObject>();
        var kitchenObjectNetworkObject = kitchenObject.GetComponent<NetworkObject>();

        if (kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject))
        {
            kitchenObjectNetworkObject.Spawn(true);
            kitchenObject.SetKitchenObjecParent(kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        if (kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject))
        {
            ClearKicthenObjectOnParantClientRpc(kitchenObjectNetworkObjectReference);
            kitchenObjectNetworkObject.GetComponent<KitchenObject>().DestroySelf();            
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
        NetworkManager.Singleton.ConnectionApprovalCallback += OnNetworkManagerConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkManagerClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkManagerClientDisconnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkManagerDisconnectCallback;
        NetworkManager.Singleton.StartClient();
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

    public Color GetPlayerColor(int colorId) =>
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
        OnNetworkManagerClientDisconnectedCallback(clientId);
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

    private void OnNetworkManagerClientConnectedCallback(ulong clientId)
    {
        playerDatasNetworkList.Add(new PlayerData { 
            clientId = clientId,
            colorId = GetFirtsUnusedColorId()
        });
    }

    private void OnNetworkManagerClientDisconnectedCallback(ulong clientId)
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

    private void OnNetworkManagerDisconnectCallback(ulong obj) =>
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);

    private void OnNetworkManagerConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
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
