using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;

    public event EventHandler OnCut;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

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
                    player.GetKitchenObject().SetKitchenObjecParent(this);

                    CleanCuttingCount();
                    InvokeOnProgressChanged(0);
                }
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjecParent(player);

                CleanCuttingCount();
                InvokeOnProgressChanged(0);
            }
            else
            {
                if (player.GetKitchenObject().TryGetPlate(out var plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectScriptableObject()))
                    {
                        GetKitchenObject().DesttoySelf();
                    }
                }
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
                CuttingCounter.OnAnyCut?.Invoke(this, EventArgs.Empty);

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
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            ProgressNormalized = progressNormalized
        });
    }

    private bool HasRecipeWithInput(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        GetCuttingRecipe(kitchenObjectScriptableObject) != null;

    private CuttingRecipeScriptableObject GetCuttingRecipe(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        Array.Find(cuttingRecipeScriptableObjects, cuttingRecipeScriptableObject =>
                cuttingRecipeScriptableObject.input.Equals(kitchenObjectScriptableObject));

}
