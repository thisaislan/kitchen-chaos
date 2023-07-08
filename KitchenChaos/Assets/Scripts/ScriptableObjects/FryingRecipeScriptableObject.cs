using UnityEngine;

[CreateAssetMenu(fileName = "FryingRecipe", menuName = "Kitchen Chaos/FryingRecipe")]
public class FryingRecipeScriptableObject : ScriptableObject
{

    public KitchenObjectScriptableObject input;
    public KitchenObjectScriptableObject output;
    public float fryingTimerMax;

}
