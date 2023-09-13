using System;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{

    [SerializeField] protected GameObject topPoint;

    private KitchenObject kitchenObject;

    public void ClearKitchenObject() =>
        kitchenObject = null;

    public Transform GetKitchenObjectFollowTransform() =>
        topPoint.transform;

    public bool HasKitchenObject() =>
        kitchenObject != null;

    public void SetKitchenObject(KitchenObject kitchenObject) =>
        this.kitchenObject = kitchenObject;

    public KitchenObject GetKitchenObject() =>
        kitchenObject;

    public NetworkObject GetNetworkObject()
    {
        //TODO - change this later
        return null;
    }

    public virtual void Interact(Player player) { }

    public virtual void InteractAlternate(Player player) { }

}
