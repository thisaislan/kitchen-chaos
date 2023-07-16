using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;

    private const string MUSIC_VULOME = "music_volume";

    public float volume { get; private set; } = 0.3f;

    private void Awake()
    {
        Instance = this;

        volume = PlayerPrefs.GetFloat(MUSIC_VULOME, volume);

        audioSource.volume = volume;
    }

    public void ChangeVolume()
    {
        volume += 0.1f;

        if (volume > 1) { volume = 0; }

        audioSource.volume = volume;

        PlayerPrefs.SetFloat(MUSIC_VULOME, volume);
    }
}
