using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnNewRecipe;
    public event EventHandler OnDelivery;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListScriptableObject recipeListScriptableObject;

    public List<RecipeScriptableObject> waitingRecipeScriptableObjects { get; private set; } = new List<RecipeScriptableObject>();

    public int successfulRecipesAmount { get; private set; } = 0;

    private float spawnRecipeTimerMax = 4;

    private int waityingSpawnRecipeMax = 4;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // TODO remove this later
        //if (IsServer)
        //{
            StartCoroutine(StarSpawnRecipeTimer());
      //  }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DelivertyCorrectRecipeServerRpc(int recipeIndex) =>
        DelivertyCorrectRecipeClientRpc(recipeIndex);

    [ServerRpc(RequireOwnership = false)]
    private void DelivertyIncorrectRecipeServerRpc() =>
        DelivertyIncorrectRecipeClientRpc();

    [ClientRpc]
    private void SpawnNewRecipeClientRpc(int waitingRecipeScriptableObjectsIndex)
    {
        var waitingRecipe = recipeListScriptableObject.recipeScriptableObjects[waitingRecipeScriptableObjectsIndex];

        waitingRecipeScriptableObjects.Add(waitingRecipe);

        OnNewRecipe?.Invoke(this, EventArgs.Empty);
    }

    [ClientRpc]
    private void DelivertyCorrectRecipeClientRpc(int recipeIndex)
    {
        waitingRecipeScriptableObjects.RemoveAt(recipeIndex);

        OnDelivery?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

        successfulRecipesAmount++;
    }

    [ClientRpc]
    private void DelivertyIncorrectRecipeClientRpc() =>
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);

    public void DeliveryRecipe(PlateKitchenObject plateKitchenObject)
    {
        var recipeScriptableObjectDelivered = GetRecipeScriptableObjectDelivered(plateKitchenObject);

        if (recipeScriptableObjectDelivered != null)
        {
            DelivertyCorrectRecipeServerRpc(waitingRecipeScriptableObjects.FindIndex(
                    recipeScriptableObject => recipeScriptableObject.recipeName.Equals(recipeScriptableObjectDelivered.recipeName)
                    )
                );
        }
        else
        {
            DelivertyIncorrectRecipeServerRpc();
        }
    }

    private RecipeScriptableObject GetRecipeScriptableObjectDelivered(PlateKitchenObject plateKitchenObject)
    {
        foreach (var waitingRecipeScriptableObject in waitingRecipeScriptableObjects)
        {
            var waitingRecipeScriptableObjectsOrdered = waitingRecipeScriptableObject.kitchenObjectScriptableObjects.OrderBy(kitchenObjectScriptableObjects => kitchenObjectScriptableObjects.objectName);
            var plateKitchenObjectScriptableObjectsOrdered = plateKitchenObject.kitchenObjectScriptableObjects.OrderBy(kitchenObjectScriptableObjects => kitchenObjectScriptableObjects.objectName);

            if (waitingRecipeScriptableObjectsOrdered.SequenceEqual(plateKitchenObjectScriptableObjectsOrdered))
            {
                return waitingRecipeScriptableObject;
            }
        }

        return null;
    }

    private IEnumerator StarSpawnRecipeTimer()
    {           
        yield return new WaitUntil(() => GameManager.Instance.State == GameManager.GameState.GamePlaying);

        // TODO remove this later
        if (!IsHost) {
            yield return null;
        }

        while (true)
        {   
            yield return new WaitForSeconds(spawnRecipeTimerMax);

            if (waitingRecipeScriptableObjects.Count < waityingSpawnRecipeMax)
            {
                SpawnNewRecipeClientRpc(UnityEngine.Random.Range(0, recipeListScriptableObject.recipeScriptableObjects.Count));                
            }
        }
    }

}