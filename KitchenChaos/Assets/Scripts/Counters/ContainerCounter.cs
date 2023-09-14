using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter, IKitchenObjectParent
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectScriptableObject, player);

            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() =>
        InteractLogicClientRpc();

    [ClientRpc]
    private void InteractLogicClientRpc() =>
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);

}
