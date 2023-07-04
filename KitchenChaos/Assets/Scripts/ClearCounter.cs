using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent
{

    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;
    [SerializeField] private GameObject topPoint;

    private KitchenObject kitchenObject;

    public void ClearKitchenObject() =>
        kitchenObject = null;

    public Transform GetKitchenObjectFollowTransform() =>
        topPoint.transform;

    public bool HasKitchenObject() =>
        kitchenObject != null;

    public void SetKitchenObject(KitchenObject kitchenObject) =>
        this.kitchenObject = kitchenObject;

    public void Interact(Player player)
    {
        if (kitchenObject == null)
        {
            var kitchenObjectNewInstance = Instantiate(kitchenObjectScriptableObject.prefab, topPoint.transform);
            kitchenObjectNewInstance.transform.localPosition = Vector3.zero;

            kitchenObject = kitchenObjectNewInstance.GetComponent<KitchenObject>();
            kitchenObject.SetKitchenObjecPatent(this);
        }
        else
        {
            kitchenObject.SetKitchenObjecPatent(player);
        }

    }

}
