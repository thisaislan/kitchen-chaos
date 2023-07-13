using UnityEngine;

[CreateAssetMenu(fileName = "BurningRecipe", menuName = "Kitchen Chaos/Burning Recipe")]
public class BurningRecipeScriptableObject : ScriptableObject
{

    public KitchenObjectScriptableObject input;
    public KitchenObjectScriptableObject output;
    public float burningTimerMax;

}
