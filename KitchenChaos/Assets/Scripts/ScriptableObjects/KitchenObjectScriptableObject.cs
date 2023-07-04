using UnityEngine;

[CreateAssetMenu(fileName = "KitchenObject", menuName = "Kitchen Chaos/KitchenObject")]
public class KitchenObjectScriptableObject : ScriptableObject
{
    public GameObject prefab;
    public Sprite image;
    public string objectName;
}
