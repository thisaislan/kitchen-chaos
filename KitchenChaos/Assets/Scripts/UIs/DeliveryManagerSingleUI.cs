using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI recipeNameText;

    [SerializeField] private GameObject iconContainer;
    [SerializeField] private GameObject iconTemplate;

    public void SetRecipeScriptableObject(RecipeScriptableObject recipeScriptableObject)
    {
        // 1 here to avoid get the parent transform
        var index = 1;
        var allIcons = iconContainer.GetComponentsInChildren<Transform>(false);
       
        recipeNameText.text = recipeScriptableObject.name;

        foreach (var kitchenObjectScriptableObject in recipeScriptableObject.kitchenObjectScriptableObjects)
        {
            var iconTemplateNewInstance = index < allIcons.Length
                ? allIcons[index]
                : Instantiate(iconTemplate, iconContainer.transform)
                .GetComponent<Transform>();
            
            iconTemplateNewInstance.GetComponent<Image>().sprite = kitchenObjectScriptableObject.image;
            iconTemplateNewInstance.gameObject.SetActive(true);

            index++;
        }

        for (; index < allIcons.Length; index++)
        {
            Destroy(allIcons[index].gameObject);
            index++;
        }   
    }

}