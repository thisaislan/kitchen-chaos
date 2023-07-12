using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeList", menuName = "Kitchen Chaos/Recipe List")]
public class RecipeListScriptableObject : ScriptableObject
{
    
    public List<RecipeScriptableObject> recipeScriptableObjects;

}
