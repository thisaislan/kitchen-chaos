using System;
using System.Collections;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler OnStoveTurnOn;
    public event EventHandler OnStoveTurnOff;


    [SerializeField] private FryingRecipeScriptableObject[] fryingRecipeScriptableObjects;
    [SerializeField] private BurningRecipeScriptableObject[] burningRecipeScriptableObjects;

    private float fryingTimer;
    private float burningTimer;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasFryingRecipeWithInput(player.GetKitchenObject().GetKitchenObjectScriptableObject()))
                {
                    player.GetKitchenObject().SetKitchenObjecPatent(this);
                    
                    CleanFryingTimer();
                    BurningFryingTimer();
                    InvokeOnProgressChanged(0);

                    OnStoveTurnOn?.Invoke(this, EventArgs.Empty);

                    StartCoroutine(StartFrying());
                }
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjecPatent(player);
                
                CleanFryingTimer();
                BurningFryingTimer();
                InvokeOnProgressChanged(0);

                OnStoveTurnOff?.Invoke(this, EventArgs.Empty);

                StopAllCoroutines();
            }
        }
    }

    private void CleanFryingTimer() =>
        fryingTimer = 0;
    private void BurningFryingTimer() =>
        burningTimer = 0;

    private bool HasFryingRecipeWithInput(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        GetFryingRecipe(kitchenObjectScriptableObject) != null;

    private FryingRecipeScriptableObject GetFryingRecipe(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        Array.Find(fryingRecipeScriptableObjects, fryingRecipeScriptableObject =>
                fryingRecipeScriptableObject.input.Equals(kitchenObjectScriptableObject));

    private BurningRecipeScriptableObject GetBurningRecipe(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        Array.Find(burningRecipeScriptableObjects, burningRecipeScriptableObject =>
                burningRecipeScriptableObject.input.Equals(kitchenObjectScriptableObject));

    private void InvokeOnProgressChanged(float progressNormalized)
    {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            ProgressNormalized = progressNormalized
        });
    }

    private IEnumerator StartFrying()
    {
        while (true)
        {
            var fryingRecipeScriptableObject = GetFryingRecipe(GetKitchenObject().GetKitchenObjectScriptableObject());

            fryingTimer += Time.deltaTime;

            InvokeOnProgressChanged(fryingTimer / fryingRecipeScriptableObject.fryingTimerMax);

            if (fryingTimer >= fryingRecipeScriptableObject.fryingTimerMax)
            {
                InvokeOnProgressChanged(0);

                GetKitchenObject().DesttoySelf();

                KitchenObject.SpawnKitchenObject(fryingRecipeScriptableObject.output, this);

                StartCoroutine(StartBurning());
                break;
            }

            yield return new WaitForEndOfFrame();
        }

    }

    private IEnumerator StartBurning()
    {
        while (true)
        {
            var burningRecipeScriptableObject = GetBurningRecipe(GetKitchenObject().GetKitchenObjectScriptableObject());
            
            burningTimer += Time.deltaTime;

            InvokeOnProgressChanged(burningTimer / burningRecipeScriptableObject.burningTimerMax);

            if (burningTimer >= burningRecipeScriptableObject.burningTimerMax)
            {
                InvokeOnProgressChanged(0);

                GetKitchenObject().DesttoySelf();

                KitchenObject.SpawnKitchenObject(burningRecipeScriptableObject.output, this);

                break;
            }

            yield return new WaitForEndOfFrame();
        }

    }

}
