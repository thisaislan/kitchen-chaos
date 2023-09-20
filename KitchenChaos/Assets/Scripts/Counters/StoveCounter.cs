using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler OnStoveTurnOn;
    public event EventHandler OnStoveTurnOff;


    [SerializeField] private FryingRecipeScriptableObject[] fryingRecipeScriptableObjects;
    [SerializeField] private BurningRecipeScriptableObject[] burningRecipeScriptableObjects;

    public bool IsBurning { get => burningTimer > 0; }

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
                    var kitchenObject = player.GetKitchenObject();

                    kitchenObject.SetKitchenObjecParent(this);

                    InteractLogicPutKitchenObjectServerRpc(KitchenGameMultiplayer.Instance.GetKitchenScriptableObjectIndex(kitchenObject.GetKitchenObjectScriptableObject()));
                }
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjecParent(player);

                InteractLogicGetKitchenObjectServerRpc();
            }
            else
            {
                if (player.GetKitchenObject().TryGetPlate(out var plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectScriptableObject()))
                    {                        
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        InteractLogicGetKitchenObjectServerRpc();
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPutKitchenObjectServerRpc(int kitchenObjectScriptableObjectIndex) =>
       InteractLogicPutKitchenObjectClientRpc(kitchenObjectScriptableObjectIndex);

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicGetKitchenObjectServerRpc() =>
       InteractLogicGetKitchenObjectClientRpc();

    [ClientRpc]
    private void InteractLogicPutKitchenObjectClientRpc(int kitchenObjectScriptableObjectIndex)
    {
        var kitchenObjectScriptableObject = KitchenGameMultiplayer.Instance.GetKitchenScriptableObjectFromIndex(kitchenObjectScriptableObjectIndex);

        CleanTimers();

        OnStoveTurnOn?.Invoke(this, EventArgs.Empty);
        StartCoroutine(StartFrying(kitchenObjectScriptableObject));
    }

    [ClientRpc]
    private void InteractLogicGetKitchenObjectClientRpc()
    {
        CleanTimers();

        OnStoveTurnOff?.Invoke(this, EventArgs.Empty);
        StopAllCoroutines();
    }

    private void CleanTimers()
    {
        CleanFryingTimer();
        BurningFryingTimer();
        InvokeOnProgressChanged(0);
    }

    private void CleanFryingTimer() =>
        fryingTimer = 0;
    private void BurningFryingTimer() =>
        burningTimer = 0;

    private bool HasFryingRecipeWithInput(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        GetFryingRecipe(kitchenObjectScriptableObject) != null;

    private FryingRecipeScriptableObject GetFryingRecipe(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        Array.Find(fryingRecipeScriptableObjects, fryingRecipeScriptableObject =>
                fryingRecipeScriptableObject.input.objectName.Equals(kitchenObjectScriptableObject.objectName));

    private BurningRecipeScriptableObject GetBurningRecipe(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        Array.Find(burningRecipeScriptableObjects, burningRecipeScriptableObject =>
                burningRecipeScriptableObject.input.objectName.Equals(kitchenObjectScriptableObject.objectName));

    private void InvokeOnProgressChanged(float progressNormalized)
    {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            ProgressNormalized = progressNormalized
        });
    }

    private IEnumerator StartFrying(KitchenObjectScriptableObject kitchenObjectScriptableObject)
    {
        while (true)
        {
            var fryingRecipeScriptableObject = GetFryingRecipe(kitchenObjectScriptableObject);

            fryingTimer += Time.deltaTime;

            InvokeOnProgressChanged(fryingTimer / fryingRecipeScriptableObject.fryingTimerMax);

            if (fryingTimer >= fryingRecipeScriptableObject.fryingTimerMax)
            {
                InvokeOnProgressChanged(0);
               
                if (IsServer)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());

                    KitchenObject.SpawnKitchenObject(fryingRecipeScriptableObject.output, this);
                }

                StartCoroutine(StartBurning(GetKitchenObject().GetKitchenObjectScriptableObject()));
                break;
            }

            yield return new WaitForEndOfFrame();
        }

    }

    private IEnumerator StartBurning(KitchenObjectScriptableObject kitchenObjectScriptableObject)
    {
        while (true)
        {
            var burningRecipeScriptableObject = GetBurningRecipe(kitchenObjectScriptableObject);
            
            burningTimer += Time.deltaTime;

            InvokeOnProgressChanged(burningTimer / burningRecipeScriptableObject.burningTimerMax);

            if (burningTimer >= burningRecipeScriptableObject.burningTimerMax)
            {
                InvokeOnProgressChanged(0);

                if (IsServer)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());

                    KitchenObject.SpawnKitchenObject(burningRecipeScriptableObject.output, this);
                }

                OnStoveTurnOff?.Invoke(this, EventArgs.Empty);

                break;
            }

            yield return new WaitForEndOfFrame();
        }

    }
    
}
