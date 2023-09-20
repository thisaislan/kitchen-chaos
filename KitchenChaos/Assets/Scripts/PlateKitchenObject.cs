using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

    public event EventHandler<OnIngredientEventArgs> OnIngridientAddedEvent;

    public class OnIngredientEventArgs : EventArgs {
        public KitchenObjectScriptableObject kitchenObjectScriptableObject;
    }

    [SerializeField] private List<KitchenObjectScriptableObject> validKitchenObjectScriptableObjects;

    public List<KitchenObjectScriptableObject> kitchenObjectScriptableObjects { get; private set; } = new();

    [ServerRpc(RequireOwnership = false)]
    private void AddIngridientServerRpc(int kitchenObjectScriptableObjectIndex) =>
        AddIngridientClientRpc(kitchenObjectScriptableObjectIndex);

    [ClientRpc]
    private void AddIngridientClientRpc(int kitchenObjectScriptableObjectIndex)
    {
        var kitchenObjectScriptableObject = KitchenGameMultiplayer.Instance.GetKitchenScriptableObjectFromIndex(kitchenObjectScriptableObjectIndex);

        kitchenObjectScriptableObjects.Add(kitchenObjectScriptableObject);

        OnIngridientAddedEvent?.Invoke(this, new OnIngredientEventArgs
        {
            kitchenObjectScriptableObject = kitchenObjectScriptableObject
        });
    }

    public bool TryAddIngredient(KitchenObjectScriptableObject kitchenObjectScriptableObject)
    {
        if (kitchenObjectScriptableObjects.Contains(kitchenObjectScriptableObject) ||
            !validKitchenObjectScriptableObjects.Contains(kitchenObjectScriptableObject))
        {
            return false;
        }

        AddIngridientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenScriptableObjectIndex(kitchenObjectScriptableObject));

        return true;
    }

}
