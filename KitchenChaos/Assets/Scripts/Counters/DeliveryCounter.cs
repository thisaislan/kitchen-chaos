public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

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
