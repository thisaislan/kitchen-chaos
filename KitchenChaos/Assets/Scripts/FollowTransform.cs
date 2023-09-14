using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform targetTransform { private get; set; }

    private void LateUpdate()
    {
        if (targetTransform == null) 
        {
            return;
        }

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }

}
