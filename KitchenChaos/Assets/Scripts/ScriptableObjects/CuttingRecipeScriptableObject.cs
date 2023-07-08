using UnityEngine;

[CreateAssetMenu(fileName = "CuttingRecipe", menuName = "Kitchen Chaos/CuttingRecipe")]
public class CuttingRecipeScriptableObject : ScriptableObject
{

    public KitchenObjectScriptableObject input;
    public KitchenObjectScriptableObject output;
    public int cuttingProgressMax;

}
