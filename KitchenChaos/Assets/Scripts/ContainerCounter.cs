using System;
using UnityEngine;

public class ContainerCounter : BaseCounter, IKitchenObjectParent
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            var kitchenObject = KitchenObject.SpawnKitchenObject(kitchenObjectScriptableObject, player);
            kitchenObject.SetKitchenObjecPatent(player);

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }

}
