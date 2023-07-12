public class DeliveryCounter : BaseCounter
{

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject()) 
        {
            if (player.GetKitchenObject().TryGetPlate(out var plateKitchenObject))
            {
                DeliveryManager.Instance.DeliveryRecipe(plateKitchenObject);
                player.GetKitchenObject().DesttoySelf();
            }
        }
    }

}
