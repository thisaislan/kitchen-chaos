using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button singleplayerplayButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        multiplayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.PlayMultplayer = true;
            Loader.LoadScene(Loader.SceneName.LoobyScene);
        });

        singleplayerplayButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.PlayMultplayer = false;
            Loader.LoadScene(Loader.SceneName.LoobyScene);
        });

        quitButton.onClick.AddListener(() => Application.Quit());

        Time.timeScale = 1;

    }
    
}
