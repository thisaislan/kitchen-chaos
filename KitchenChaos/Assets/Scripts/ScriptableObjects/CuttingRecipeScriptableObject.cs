using UnityEngine;

[CreateAssetMenu(fileName = "CuttingRecipe", menuName = "Kitchen Chaos/Cutting Recipe")]
public class CuttingRecipeScriptableObject : ScriptableObject
{

    public KitchenObjectScriptableObject input;
    public KitchenObjectScriptableObject output;
    public int cuttingProgressMax;

}
