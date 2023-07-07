using System;
using UnityEngine;

public class CuttingCounter : BaseCounter
{

    [SerializeField] private CuttingRecipeScriptableObject[] cuttingRecipeScriptableObjects;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectScriptableObject()))
                {
                    player.GetKitchenObject().SetKitchenObjecPatent(this);
                }
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjecPatent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject())
        {
            var currentKitchenObject = GetKitchenObject();
            var newCurrentKitchenObject = GetCuttingRecipe(currentKitchenObject.GetKitchenObjectScriptableObject());

            if (newCurrentKitchenObject != null)
            {
                currentKitchenObject.DesttoySelf();
                KitchenObject.SpawnKitchenObject(newCurrentKitchenObject.output, this);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        GetCuttingRecipe(kitchenObjectScriptableObject) != null;

    private CuttingRecipeScriptableObject GetCuttingRecipe(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        Array.Find<CuttingRecipeScriptableObject>(
            cuttingRecipeScriptableObjects, cuttingRecipeScriptableObject =>
                cuttingRecipeScriptableObject.input.Equals(kitchenObjectScriptableObject));

}
