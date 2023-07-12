using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{

    [SerializeField] private GameObject container;
    [SerializeField] private GameObject recipeTemplate;

    private void Start()
    {
        DeliveryManager.Instance.OnDelivery += OnDeliveryManagerDelivery;
        DeliveryManager.Instance.OnNewRecipe += OnDeliveryManagerNewRecipe;

        UpdateVisual();
    }
    private void OnDestroy()
    {
        DeliveryManager.Instance.OnDelivery -= OnDeliveryManagerDelivery;
        DeliveryManager.Instance.OnNewRecipe -= OnDeliveryManagerNewRecipe;
    }

    private void OnDeliveryManagerNewRecipe(object sender, System.EventArgs e) =>
        UpdateVisual();

    private void OnDeliveryManagerDelivery(object sender, System.EventArgs e) =>
        UpdateVisual();

    private void UpdateVisual()
    {
        var index = 0;
        var allDeliveryManagerSingleUIs = container.transform.parent.GetComponentsInChildren<DeliveryManagerSingleUI>(false);

        foreach (var waitingRecipeScriptableObject in DeliveryManager.Instance.waitingRecipeScriptableObjects)
        {
            var recipeSingleUI = index < allDeliveryManagerSingleUIs.Length
                ? allDeliveryManagerSingleUIs[index]
                : Instantiate(recipeTemplate, container.transform)
                    .GetComponent<DeliveryManagerSingleUI>();

            recipeSingleUI.SetRecipeScriptableObject(waitingRecipeScriptableObject);
            recipeSingleUI.gameObject.SetActive(true);

            index++;
        }

        for (; index < allDeliveryManagerSingleUIs.Length; index++)
        {
            Destroy(allDeliveryManagerSingleUIs[index].gameObject);
            index++;
        }

    }

}