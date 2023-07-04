using UnityEngine;

public class KitchenObject : MonoBehaviour
{

    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;

    public IKitchenObjectParent kitchenObjectParent { get; private set; }

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

}
