using UnityEngine;
using UnityEngine.UI;

public class TestLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private void Awake()
    {
        createGameButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetworkScene(Loader.SceneName.CharacterSelectScene);
        });

        joinGameButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.Instance.StartClient();
        });
    }
}
