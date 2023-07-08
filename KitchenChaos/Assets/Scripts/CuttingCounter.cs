using System;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    public event EventHandler OnCut;

    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

    public class OnProgressChangedEventArgs : EventArgs
    {
        public float ProgressNormalized;
    }


    [SerializeField] private CuttingRecipeScriptableObject[] cuttingRecipeScriptableObjects;

    private int cuttingCount;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectScriptableObject()))
                {
                    player.GetKitchenObject().SetKitchenObjecPatent(this);

                    CleanCuttingCount();
                    InvokeOnProgressChanged(0);
                }
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjecPatent(player);

                CleanCuttingCount();
                InvokeOnProgressChanged(0);
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
                cuttingCount++;

                OnCut?.Invoke(this, EventArgs.Empty);

                InvokeOnProgressChanged((float)cuttingCount / newCurrentKitchenObject.cuttingProgressMax);

                if (cuttingCount >= newCurrentKitchenObject.cuttingProgressMax)
                {
                    currentKitchenObject.DesttoySelf();
                    KitchenObject.SpawnKitchenObject(newCurrentKitchenObject.output, this);

                    CleanCuttingCount();
                }
            }
        }
    }

    private void CleanCuttingCount() =>
        cuttingCount = 0;

    private void InvokeOnProgressChanged(float progressNormalized)
    {
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            ProgressNormalized = progressNormalized
        });
    }

    private bool HasRecipeWithInput(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        GetCuttingRecipe(kitchenObjectScriptableObject) != null;

    private CuttingRecipeScriptableObject GetCuttingRecipe(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        Array.Find<CuttingRecipeScriptableObject>(
            cuttingRecipeScriptableObjects, cuttingRecipeScriptableObject =>
                cuttingRecipeScriptableObject.input.Equals(kitchenObjectScriptableObject));

}
