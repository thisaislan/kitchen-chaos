using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseUI : MonoBehaviour
{
    [SerializeField] private GameObject connectionResponseContainer;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += OnKitchenGameMultiplayerFailedToJoinGame;

        closeButton.onClick.AddListener(() => connectionResponseContainer.SetActive(false));
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= OnKitchenGameMultiplayerFailedToJoinGame;
    }

    private void OnKitchenGameMultiplayerFailedToJoinGame(object sender, System.EventArgs e)
    {       
        connectionResponseContainer.SetActive(true);
        
        message.text = NetworkManager.Singleton.DisconnectReason.Equals(string.Empty) ? 
            "Failed to connect" :
            NetworkManager.Singleton.DisconnectReason;
    }
}
