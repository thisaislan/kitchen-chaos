using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{

    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;
    [SerializeField] private FollowTransform followTransform;

    public IKitchenObjectParent kitchenObjectParent { get; private set; }

    public static void SpawnKitchenObject(
        KitchenObjectScriptableObject kitchenObjectScriptableObject,
        IKitchenObjectParent kitchenObjectParent
        )
    {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectScriptableObject, kitchenObjectParent);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) =>
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        if (kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject))
        {
            var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

            this.kitchenObjectParent?.ClearKitchenObject();
            this.kitchenObjectParent = kitchenObjectParent;
            kitchenObjectParent.SetKitchenObject(this);
            followTransform.targetTransform = kitchenObjectParent.GetKitchenObjectFollowTransform();
        }
    }

    public KitchenObjectScriptableObject GetKitchenObjectScriptableObject() =>
        kitchenObjectScriptableObject;

    public void SetKitchenObjecParent(IKitchenObjectParent kitchenObjectParent) =>
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());

    public void DesttoySelf() 
    {
        kitchenObjectParent.ClearKitchenObject();

        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject) { plateKitchenObject = this as PlateKitchenObject; }
        else { plateKitchenObject = null; }

        return plateKitchenObject != null;
    }

}