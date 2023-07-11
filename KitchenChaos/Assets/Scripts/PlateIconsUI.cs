using System;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{

    [SerializeField] PlateKitchenObject plateKitchenObject;
    [SerializeField] GameObject iconTemplate;

    private void Start()
    {
        plateKitchenObject.OnIngridientAddedEvent += OnPlateKitchenObjectIngridientAddedEvent;
    }

    private void OnDestroy()
    {
        plateKitchenObject.OnIngridientAddedEvent -= OnPlateKitchenObjectIngridientAddedEvent;
    }

    private void OnPlateKitchenObjectIngridientAddedEvent(object sender, PlateKitchenObject.OnIngredientEventArgs e) =>
        UpdateVisual();

    private void UpdateVisual()
    {
        var index = 0;
        var allIcons = iconTemplate.transform.parent.GetComponentsInChildren<PlateIconSingleUI>(false);

        foreach (var kitchenObjectScriptableObject in plateKitchenObject.kitchenObjectScriptableObjects)
        {
            var plateIconSingleUI = index < allIcons.Length 
                ? allIcons[index]
                : Instantiate(iconTemplate, transform)
                    .GetComponent<PlateIconSingleUI>();

            plateIconSingleUI.SetKitchenObjectScriptableObject(kitchenObjectScriptableObject);
            plateIconSingleUI.gameObject.SetActive(true);

            index++;
        }

        for (; index < allIcons.Length; index++)
        {
            Destroy(allIcons[index].gameObject);
            index++;
        }

    }

}
