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
            kitchenObjectNetworkObject.GetComponent<KitchenObject>().DesttoySelf();            
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

    private int GetKitchenScriptableObjectIndex(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        KichenObjectList.kitchenObjectScriptableObjectsList.IndexOf(kitchenObjectScriptableObject);

    private KitchenObjectScriptableObject GetKitchenScriptableObjectFromIndex(int kitchenObjectScriptableObjectIndex) =>
        KichenObjectList.kitchenObjectScriptableObjectsList[kitchenObjectScriptableObjectIndex];

}
