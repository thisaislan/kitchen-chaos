using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestNetworkUI : MonoBehaviour
{

    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() => { 
            NetworkManager.Singleton.StartHost();
            Debug.Log("Start as Host");
            Hide();
        });

        startClientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Start as Client");
            Hide();
        });
    }

    private void Hide() =>
        gameObject.SetActive(false);

}
