using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
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
        StartCoroutine(StarSpawnRecipeTimer());
    }

    public void DeliveryRecipe(PlateKitchenObject plateKitchenObject)
    {
        var recipeScriptableObjectDelivered = GetRecipeScriptableObjectDelivered(plateKitchenObject);

        if (recipeScriptableObjectDelivered != null)
        {
            waitingRecipeScriptableObjects.Remove(recipeScriptableObjectDelivered);

            OnDelivery?.Invoke(this, EventArgs.Empty);
            OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

            successfulRecipesAmount++;
        }
        else
        {
            OnRecipeFailed?.Invoke(this, EventArgs.Empty);
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
        while (true)
        {
            yield return new WaitForSeconds(spawnRecipeTimerMax);            

            if (waitingRecipeScriptableObjects.Count < waityingSpawnRecipeMax)
            {
                var waitingRecipe = recipeListScriptableObject.recipeScriptableObjects[UnityEngine.Random.Range(0, recipeListScriptableObject.recipeScriptableObjects.Count)];

                waitingRecipeScriptableObjects.Add(waitingRecipe);

                OnNewRecipe?.Invoke(this, EventArgs.Empty);
            }
        }
    }

}