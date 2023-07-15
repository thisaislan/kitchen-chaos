using System;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{

    [Serializable]
    public struct KitchenObjectScriptableObjectGameObject
    {
        public KitchenObjectScriptableObject kitchenObjectScriptableObject;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;

    [SerializeField] private KitchenObjectScriptableObjectGameObject[] kitchenObjectScriptableObjectGameObjects;

    private void Start()
    {
        plateKitchenObject.OnIngridientAddedEvent += OnPlateKitchenObjectIngridientAddedEvent;
    }

    private void OnDestroy()
    {
        plateKitchenObject.OnIngridientAddedEvent -= OnPlateKitchenObjectIngridientAddedEvent;
    }

    private void OnPlateKitchenObjectIngridientAddedEvent(object sender, PlateKitchenObject.OnIngredientEventArgs e) =>
        GetKitchenObjectScriptableObjectGameObject(e.kitchenObjectScriptableObject).gameObject.SetActive(true);

    private KitchenObjectScriptableObjectGameObject GetKitchenObjectScriptableObjectGameObject(KitchenObjectScriptableObject kitchenObjectScriptableObject) =>
        Array.Find(kitchenObjectScriptableObjectGameObjects, kitchenObjectScriptableObjectGameObject =>
                kitchenObjectScriptableObjectGameObject.kitchenObjectScriptableObject.Equals(kitchenObjectScriptableObject));

}
