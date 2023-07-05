using UnityEngine;

public class ClearCounter : BaseCounter
{

    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjecPatent(this);
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjecPatent(player);
            }
        }
    }

}
