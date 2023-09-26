using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;

    [SerializeField] private GameObject pauseConteiner;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => GameManager.Instance.UnpauseGame());

        optionsButton.onClick.AddListener(() =>
        {
            pauseConteiner.SetActive(false);

            OptionsUI.Instance.Show(() =>
            {
                resumeButton.Select();
                pauseConteiner.SetActive(true);
            });
        });

        mainMenuButton.onClick.AddListener(() => Loader.LoadScene(Loader.SceneName.MainMenuScene));
    }

    void Start()
    {
        GameManager.Instance.OnLocalGamePaused += OnGameManagerLocalGamePause;
        GameManager.Instance.OnLocalGameUnpaused += OnGameManagerLocalGameUnpause;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLocalGamePaused -= OnGameManagerLocalGamePause;
        GameManager.Instance.OnLocalGameUnpaused -= OnGameManagerLocalGameUnpause;
    }

    private void OnGameManagerLocalGamePause(object sender, System.EventArgs e)
    {
        pauseConteiner.SetActive(true);

        if (GameManager.Instance.IsLocalGamePaused)
        {
            resumeButton.Select();
        }
    }

    private void OnGameManagerLocalGameUnpause(object sender, System.EventArgs e) =>
        pauseConteiner.SetActive(false);

}
