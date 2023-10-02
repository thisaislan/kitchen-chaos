using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] private GameObject connectionContainer;

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame += OnKitchenGameMultiplayerTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += OnKitchenGameMultiplayerFailedToJoinGame;
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= OnKitchenGameMultiplayerTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= OnKitchenGameMultiplayerFailedToJoinGame;
    }

    private void OnKitchenGameMultiplayerFailedToJoinGame(object sender, System.EventArgs e) =>
        connectionContainer.SetActive(false);

    private void OnKitchenGameMultiplayerTryingToJoinGame(object sender, System.EventArgs e) =>
        connectionContainer.SetActive(true);
}
