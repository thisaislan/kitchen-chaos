using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipRefsScriptableObject audioClipRefsScriptableObjects;

    private const string SOUND_VULOME = "sound_volume";

    public float volume { get; private set; }  = 1f;

    private void Awake()
    { 
        Instance = this;

        volume = PlayerPrefs.GetFloat(SOUND_VULOME, volume);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += OnDeliveryManagerRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += OnDeliveryManagerRecipeFailed;
        CuttingCounter.OnAnyCut += OnCuttingCounterOnAnyCut;
        Player.Instance.OnPickedSomething += OnPlayerPickedSomething;
        ClearCounter.OnAnyPlaceKitchenObject += OnClearCounterPlaceKitchenObject;
        TrashCounter.OnAnyObjectTrashed += OnTrashCounterAnyObjectTrashed;
    }

    private void OnDestroy()
    {
        DeliveryManager.Instance.OnRecipeSuccess -= OnDeliveryManagerRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed -= OnDeliveryManagerRecipeFailed;
        CuttingCounter.OnAnyCut -= OnCuttingCounterOnAnyCut;
        Player.Instance.OnPickedSomething -= OnPlayerPickedSomething;
        ClearCounter.OnAnyPlaceKitchenObject -= OnClearCounterPlaceKitchenObject;
        TrashCounter.OnAnyObjectTrashed -= OnTrashCounterAnyObjectTrashed;
    }

    private void OnTrashCounterAnyObjectTrashed(object sender, System.EventArgs e) =>
        PlaySound(audioClipRefsScriptableObjects.trash, (sender as BaseCounter).transform.position);

    private void OnClearCounterPlaceKitchenObject(object sender, System.EventArgs e) =>
        PlaySound(audioClipRefsScriptableObjects.objectDrop, (sender as BaseCounter).transform.position);

    private void OnPlayerPickedSomething(object sender, System.EventArgs e) =>
        PlaySound(audioClipRefsScriptableObjects.objectPickup, Player.Instance.transform.position);

    private void OnCuttingCounterOnAnyCut(object sender, System.EventArgs e) =>
        PlaySound(audioClipRefsScriptableObjects.chop, (sender as BaseCounter).transform.position);

    private void OnDeliveryManagerRecipeFailed(object sender, System.EventArgs e) =>
        PlaySound(audioClipRefsScriptableObjects.deliveryFail, DeliveryCounter.Instance.transform.position);

    private void OnDeliveryManagerRecipeSuccess(object sender, System.EventArgs e) =>
        PlaySound(audioClipRefsScriptableObjects.deliverySuccess, DeliveryCounter.Instance.transform.position);

    private void PlaySound(AudioClip[] audioClips, Vector3 position, float volume = 1f) =>
        PlaySound(audioClips[UnityEngine.Random.Range(0, audioClips.Length)], position, volume);

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f) =>
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);

    public void PlayFootstepSound(Vector3 position, float volume = 1f) =>
       PlaySound(audioClipRefsScriptableObjects.footstep, position, volume);

    public void ChangeVolume()
    {
        volume += 0.1f;

        if (volume > 1) { volume = 0; }

        PlayerPrefs.SetFloat(SOUND_VULOME, volume);        
    }

}
