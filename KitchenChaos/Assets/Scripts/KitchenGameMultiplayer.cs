using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KichenObjectList KichenObjectList;

    private void Awake()
    {
        Instance = this;
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
        NetworkManager.Singleton.StartClient();
    }

    private void OnNetworkManagerConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (GameManager.Instance.State.Value == GameManager.GameState.WaitingToStart)
        {
            connectionApprovalResponse.Approved = true;
            connectionApprovalResponse.CreatePlayerObject = true;
        }
        else
        { 
            connectionApprovalResponse.Approved = false;
        }
    }
}
