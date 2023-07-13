using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipRefs", menuName = "Kitchen Chaos/Audio Clip Refs")]
public class AudioClipRefsScriptableObject : ScriptableObject
{

    public AudioClip[] chop;
    public AudioClip[] deliveryFail;
    public AudioClip[] deliverySuccess;
    public AudioClip[] footstep;
    public AudioClip[] objectDrop;
    public AudioClip[] objectPickup;
    public AudioClip[] stoveSizzle;
    public AudioClip[] trash;
    public AudioClip[] warning;

}
