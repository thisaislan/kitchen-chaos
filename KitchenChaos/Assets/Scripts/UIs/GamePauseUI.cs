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
        resumeButton.onClick.AddListener(() => GameManager.Instance.ToglePauseGame());

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

    void Start() =>
        GameManager.Instance.OnTogglePause += OnGameManagerTogglePause;

    private void OnGameManagerTogglePause(object sender, System.EventArgs e)
    {
        pauseConteiner.SetActive(GameManager.Instance.IsGamePaused);

        if (GameManager.Instance.IsGamePaused)
        {
            resumeButton.Select();
        }
    }
    
}
