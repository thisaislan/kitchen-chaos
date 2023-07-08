using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IHasProgress;

public class PlatesCounter : BaseCounter
{

    public event EventHandler OnPlateSpwned;
    public event EventHandler OnPlateRemove;

    [SerializeField] KitchenObjectScriptableObject plateScriptableObject;

    private float spawnPlateTimer;
    private int platesSpawnedAmount;

    private float spawnPlateTimerMax = 4;
    private int platesSpawnedAmountMax = 4;
    
    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;

        if (spawnPlateTimer >= spawnPlateTimerMax)
        {
            spawnPlateTimer = 0;

            if (platesSpawnedAmount < platesSpawnedAmountMax)
            {
                platesSpawnedAmount++;

                OnPlateSpwned?.Invoke(this, EventArgs.Empty);
            }            
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (platesSpawnedAmount > 0)
            {
                platesSpawnedAmount--;

                KitchenObject.SpawnKitchenObject(plateScriptableObject, player);

                OnPlateRemove?.Invoke(this, EventArgs.Empty);
            }
        }
    }

}
