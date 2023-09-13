using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent
{

    public Transform GetKitchenObjectFollowTransform();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public void ClearKitchenObject();

    public bool HasKitchenObject();

    public KitchenObject GetKitchenObject();

    public NetworkObject GetNetworkObject();

}
