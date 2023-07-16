using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{

    public static OptionsUI Instance { get; private set; }

    [SerializeField] GameObject optionsConteiner;
        
    [SerializeField] private Button soundEffectsMenuButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private TextMeshProUGUI soundEffectsMenuButtonText;
    [SerializeField] private TextMeshProUGUI musicButtonText;

    private void Awake()
    {
        Instance = this;

        soundEffectsMenuButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisuals();
        });

        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisuals();
        });

        closeButton.onClick.AddListener(() => Hide());
    }

    private void Start()
    {
        UpdateVisuals();

        GameManager.Instance.OnTogglePause += OnGameManagerTogglePause;
    }

    private void OnGameManagerTogglePause(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePaused) { Hide(); }
    }

    private void UpdateVisuals()
    {
        soundEffectsMenuButtonText.text = $"Sound Effects: {Mathf.Round(SoundManager.Instance.volume * 10f)}";
        musicButtonText.text = $"Music: {Mathf.Round(MusicManager.Instance.volume * 10f)}";
    }

    public void Show() =>
        optionsConteiner.SetActive(true);

    public void Hide() =>
        optionsConteiner.SetActive(false);

}
