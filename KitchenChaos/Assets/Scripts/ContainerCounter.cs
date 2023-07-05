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
            var kitchenObjectNewInstance = Instantiate(kitchenObjectScriptableObject.prefab);
            kitchenObjectNewInstance.GetComponent<KitchenObject>().SetKitchenObjecPatent(player);

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }

}
