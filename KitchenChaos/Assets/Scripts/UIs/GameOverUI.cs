using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] private GameObject gameOverConteiner;
    [SerializeField] private TextMeshProUGUI recipesDelivered;
    [SerializeField] private Button playAgain;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += OnGameManagerStateChanged;

        playAgain.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.Shutdown();
            Loader.LoadScene(Loader.SceneName.MainMenuScene);
        });
    }

    private void OnDestroy() =>
        GameManager.Instance.OnStateChanged -= OnGameManagerStateChanged;

    private void OnGameManagerStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.State.Value == GameManager.GameState.GameOver)
        {
            recipesDelivered.text = DeliveryManager.Instance.successfulRecipesAmount.ToString();
            gameOverConteiner.SetActive(true);
        }

    }
}
