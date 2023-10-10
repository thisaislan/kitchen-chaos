using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject lobbyListContainer;
    [SerializeField] private GameObject lobbyTemplate;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;

    private void Awake()
    {
        joinCodeInputField.text = "CODE";
        playerNameInputField.text = "PLAYER NAME";

        mainMenuButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.LeaveLobby();
            Loader.LoadScene(Loader.SceneName.MainMenuScene);
        });

        createLobbyButton.onClick.AddListener(() => {
            lobbyCreateUI.Show();
        });

        quickJoinButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.QuickJoin();
        });

        joinCodeButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.JoinWithCode(joinCodeInputField.text);
        });
    }

    private void Start()
    {
        playerNameInputField.text = KitchenGameMultiplayer.Instance.PlayerName;

        playerNameInputField.onValueChanged.AddListener((string newText) => 
        {
            KitchenGameMultiplayer.Instance.PlayerName = newText;
        });

        KitchenGameLobby.Instance.OnLobbyListChanged += OnKitchenGameLobbyListChanged;

        UpdateLobbyLits(new List<Lobby>());
    }

    private void OnDestroy() =>
        KitchenGameLobby.Instance.OnLobbyListChanged -= OnKitchenGameLobbyListChanged;

    private void OnKitchenGameLobbyListChanged(object sender, KitchenGameLobby.OnLobbyListChangedEventArgs listChangedEventArgs) =>
        UpdateLobbyLits(listChangedEventArgs.Lobbies);

    private void UpdateLobbyLits(List<Lobby> lobbies)
    {
        foreach (Transform child in lobbyListContainer.transform)
        {
            if (child.gameObject.name.Equals(lobbyTemplate.name)) { continue; }
            Destroy(child.gameObject);
        }

        foreach (var lobby in lobbies)
        {
            var lobbyTemplateNewInstance = Instantiate(lobbyTemplate, lobbyListContainer.transform);            
            lobbyTemplateNewInstance.gameObject.SetActive(true);
            lobbyTemplateNewInstance.GetComponent<LobbyListSibgleUI>().SetLobby(lobby);
        }
    }
}
