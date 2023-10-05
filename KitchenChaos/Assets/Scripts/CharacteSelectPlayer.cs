using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacteSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyTextGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;

    private void Awake()
    {
        kickButton.onClick.AddListener(() => 
        {
            var playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFormPlayerIndex(playerIndex);
            KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        UpdatePlayer();

        KitchenGameMultiplayer.Instance.OnPlayerDatasNetworkListChangedEvent += OnKitchenGameMultiplayerPlayerDatasNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += OnCharacterSlectReadyChanged;
        
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && playerIndex != 0);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDatasNetworkListChangedEvent -= OnKitchenGameMultiplayerPlayerDatasNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged -= OnCharacterSlectReadyChanged;
    }

    private void OnCharacterSlectReadyChanged(object sender, System.EventArgs e) =>
        UpdatePlayer();

    private void OnKitchenGameMultiplayerPlayerDatasNetworkListChanged(object sender, System.EventArgs e) =>
        UpdatePlayer();

    private void UpdatePlayer()
    {
        if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            var playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFormPlayerIndex(playerIndex);
            readyTextGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide(); 
        }
    }

    private void Show() =>
        gameObject.SetActive(true);

    private void Hide() =>
        gameObject.SetActive(false);
}
