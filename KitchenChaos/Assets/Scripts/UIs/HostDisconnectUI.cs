using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
{

    [SerializeField] private GameObject hostDisconnectConteiner;
    [SerializeField] private Button playAgain;

    void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkClientDisconnect;

        playAgain.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.LoadScene(Loader.SceneName.MainMenuScene);
        });
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnNetworkClientDisconnect;
        }
    }

    private void OnNetworkClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            hostDisconnectConteiner.SetActive(true);
        }
    }
        
}
