using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{

    [SerializeField] private GameObject container;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;

    private void Awake()
    {
        lobbyNameInputField.text = "LobbyName";

        createPublicButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
        });

        createPrivateButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void Show() =>
        container.SetActive(true);

    private void Hide() =>
        container.SetActive(false);

}
