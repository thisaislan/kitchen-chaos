using System.Collections.Generic;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary = new Dictionary<ulong, bool>();

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerReady() =>
        SetPlayerReadyServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                return;
            }
        }

        Loader.LoadNetworkScene(Loader.SceneName.GameScene);
    }
}
