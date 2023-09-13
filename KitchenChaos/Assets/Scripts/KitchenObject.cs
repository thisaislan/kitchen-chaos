using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{

    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;

    public IKitchenObjectParent kitchenObjectParent { get; private set; }

    public static void SpawnKitchenObject(
        KitchenObjectScriptableObject kitchenObjectScriptableObject,
        IKitchenObjectParent kitchenObjectParent
        )
    {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectScriptableObject, kitchenObjectParent);
    }

    public KitchenObjectScriptableObject GetKitchenObjectScriptableObject() =>
        kitchenObjectScriptableObject;

    public void SetKitchenObjecParent(IKitchenObjectParent kitchenObjectParent)
    {
        this.kitchenObjectParent?.ClearKitchenObject();

        this.kitchenObjectParent = kitchenObjectParent;

        kitchenObjectParent.SetKitchenObject(this);
        
       // transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
       // transform.localPosition = Vector3.zero;
    }

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