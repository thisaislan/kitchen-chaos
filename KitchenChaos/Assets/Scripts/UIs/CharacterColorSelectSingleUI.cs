using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObeject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDatasNetworkListChangedEvent += OnKitchenGameMultiplayerPlayerDatasNetworkListChangedEvent;
        image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
        UpdateIsSelectColor();
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDatasNetworkListChangedEvent -= OnKitchenGameMultiplayerPlayerDatasNetworkListChangedEvent;
    }

    private void OnKitchenGameMultiplayerPlayerDatasNetworkListChangedEvent(object sender, System.EventArgs e) =>
        UpdateIsSelectColor();

    private void UpdateIsSelectColor() =>
        selectedGameObeject.SetActive(KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId);
    
}
