using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] GameObject gameOverConteiner;
    [SerializeField] TextMeshProUGUI recipesDelivered;

    void Start() =>
        GameManager.Instance.OnStateChanged += OnGameManagerStateChanged;

    private void OnGameManagerStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.State == GameManager.GameState.GameOver)
        {
            recipesDelivered.text = DeliveryManager.Instance.successfulRecipesAmount.ToString();
            gameOverConteiner.SetActive(true);
        }

    }
}
