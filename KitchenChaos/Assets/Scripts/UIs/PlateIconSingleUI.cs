using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingleUI : MonoBehaviour
{

    [SerializeField] private Image image;

    public void SetKitchenObjectScriptableObject(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        image.sprite = kitchenObjectScriptableObject.image;

}
