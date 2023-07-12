using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Kitchen Chaos/Recipe")]
public class RecipeScriptableObject : ScriptableObject
{

    public List<KitchenObjectScriptableObject> kitchenObjectScriptableObjects;
    public string recipeName;
    
}
