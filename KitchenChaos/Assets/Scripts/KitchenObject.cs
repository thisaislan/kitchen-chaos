using UnityEngine;

public class KitchenObject : MonoBehaviour
{

    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;

    public IKitchenObjectParent kitchenObjectParent { get; private set; }

    public static KitchenObject SpawnKitchenObject(
        KitchenObjectScriptableObject kitchenObjectScriptableObject,
        IKitchenObjectParent kitchenObjectParent
        )
    {
        var kitchenObject = Instantiate(kitchenObjectScriptableObject.prefab).GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjecPatent(kitchenObjectParent);

        return kitchenObject;
    }

    public KitchenObjectScriptableObject GetKitchenObjectScriptableObject() =>
        kitchenObjectScriptableObject;

    public void SetKitchenObjecPatent(IKitchenObjectParent kitchenObjectParent)
    {
        this.kitchenObjectParent?.ClearKitchenObject();

        this.kitchenObjectParent = kitchenObjectParent;

        kitchenObjectParent.SetKitchenObject(this);
        
        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
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