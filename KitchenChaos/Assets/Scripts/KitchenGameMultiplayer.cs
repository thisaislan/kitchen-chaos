using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    private const int MAX_NUMBER_OF_PLAYERS = 4;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;

    [SerializeField] private KichenObjectList KichenObjectList;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        KichenObjectList.kitchenObjectScriptableObjectsList.IndexOf(kitchenObjectScriptableObject);

    public KitchenObjectScriptableObject GetKitchenScriptableObjectFromIndex(int kitchenObjectScriptableObjectIndex) =>
        KichenObjectList.kitchenObjectScriptableObjectsList[kitchenObjectScriptableObjectIndex];

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += OnNetworkManagerConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkManagerDisconnectCallback;
        NetworkManager.Singleton.StartClient();
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
