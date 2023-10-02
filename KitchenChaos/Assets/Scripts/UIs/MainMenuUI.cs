using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() => Loader.LoadScene(Loader.SceneName.LoobyScene));

        quitButton.onClick.AddListener(() => Application.Quit());

        Time.timeScale = 1;

    }
    
}
