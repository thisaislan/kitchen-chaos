using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += OnKitchenGameMultiplayerFailedToJoinGame;

        KitchenGameLobby.Instance.OnCreateLobbStarted += OnKitchenGameLobbyCreateStarted;
        KitchenGameLobby.Instance.OnCreateLobbFailed += OnKitchenGameLobbyCreateFailed;
        KitchenGameLobby.Instance.OnJoinStarted += OnKitchenGameLobbyJoinStarted;
        KitchenGameLobby.Instance.OnJoinFailed += OnKitchenGameLobbyJoinFailed;
        KitchenGameLobby.Instance.OnQuickJoinFailed += OnKitchenGameLobbyQuickJoinFailed;

        closeButton.onClick.AddListener(() => container.SetActive(false));
    }


    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= OnKitchenGameMultiplayerFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbStarted -= OnKitchenGameLobbyCreateStarted;
        KitchenGameLobby.Instance.OnCreateLobbFailed -= OnKitchenGameLobbyCreateFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= OnKitchenGameLobbyJoinStarted;
        KitchenGameLobby.Instance.OnJoinFailed -= OnKitchenGameLobbyJoinFailed;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= OnKitchenGameLobbyQuickJoinFailed;
    }

    private void OnKitchenGameMultiplayerFailedToJoinGame(object sender, System.EventArgs e) =>
        ShowMessage(NetworkManager.Singleton.DisconnectReason.Equals(string.Empty) ?
            "Failed to connect" :
            NetworkManager.Singleton.DisconnectReason);

    private void OnKitchenGameLobbyCreateStarted(object sender, System.EventArgs e) =>
        ShowMessage("Creating Lobby...");

    private void OnKitchenGameLobbyCreateFailed(object sender, System.EventArgs e) =>
        ShowMessage("Failed to create Lobby...");

    private void OnKitchenGameLobbyJoinStarted(object sender, System.EventArgs e) =>
        ShowMessage("Joining Lobby...");

    private void OnKitchenGameLobbyJoinFailed(object sender, System.EventArgs e) =>
        ShowMessage("Failed to join  Lobby...");

    private void OnKitchenGameLobbyQuickJoinFailed(object sender, System.EventArgs e) =>
        ShowMessage("Could not find a Lobby to quick join...");

    private void ShowMessage(string messageText)
    {
        container.SetActive(true);

        message.text = messageText;
    }
}
