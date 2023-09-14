using System;
using Unity.Netcode;
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
        if (!IsServer) { return; }

        if (GameManager.Instance.State == GameManager.GameState.GamePlaying)
        {
            spawnPlateTimer += Time.deltaTime;

            if (spawnPlateTimer >= spawnPlateTimerMax)
            {
                spawnPlateTimer = 0;

                if (platesSpawnedAmount < platesSpawnedAmountMax)
                {
                    SpawnPlateServerRpc();
                }
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (platesSpawnedAmount > 0)
            {
                KitchenObject.SpawnKitchenObject(plateScriptableObject, player);

                InteractLogicServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc() =>
        SpawnPlateClientRpc();

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawnedAmount++;

        OnPlateSpwned?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() =>
    InteractLogicClientRpc();

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        platesSpawnedAmount--;

        OnPlateRemove?.Invoke(this, EventArgs.Empty);
    }


}
