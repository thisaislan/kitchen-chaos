using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

    public event EventHandler<OnIngredientEventArgs> OnIngridientAddedEvent;

    public class OnIngredientEventArgs : EventArgs {
        public KitchenObjectScriptableObject kitchenObjectScriptableObject;
    }

    [SerializeField] private List<KitchenObjectScriptableObject> validJitchenObjectScriptableObjects;

    public List<KitchenObjectScriptableObject> kitchenObjectScriptableObjects { get; private set; } = new();

    public bool TryAddIngredient(KitchenObjectScriptableObject kitchenObjectScriptableObject)
    {
        if (kitchenObjectScriptableObjects.Contains(kitchenObjectScriptableObject) ||
            !validJitchenObjectScriptableObjects.Contains(kitchenObjectScriptableObject))
        {
            return false;
        }

        kitchenObjectScriptableObjects.Add(kitchenObjectScriptableObject);

        OnIngridientAddedEvent?.Invoke(this, new OnIngredientEventArgs
        {
            kitchenObjectScriptableObject = kitchenObjectScriptableObject
        });

        return true;
    }

}
