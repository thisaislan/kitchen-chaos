using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{

    [SerializeField] private BaseCounter counter;
    [SerializeField] private GameObject[] visualGameObjects;

    void Start()
    {
        if (Player.LocalInstance != null)
        {
            SetOnPlayerSelectedCounterChanged();
        }
        else
        {
            Player.OnAnyPlayerSpawn += OnPlayerAnyPlayerSpawn;
        }
    }

    private void OnPlayerAnyPlayerSpawn(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            SetOnPlayerSelectedCounterChanged();
            RemoveSetOnPlayerAnyPlayerSpawn();
        }
    }

    private void OnDestroy()
    {
        Player.OnAnyPlayerSpawn -= OnPlayerAnyPlayerSpawn;
        RemoveSetOnPlayerAnyPlayerSpawn();
    }

    private void OnPlayerSelectedCounterChanged(object sender, Player.OnSelectedChangedEventArgs playerEventArgs) =>
        SetVisualActive(playerEventArgs.SelectedCounter == counter);

    private void SetOnPlayerSelectedCounterChanged() =>
        Player.LocalInstance.OnSelectedCounterChanged += OnPlayerSelectedCounterChanged;

    private void RemoveSetOnPlayerAnyPlayerSpawn() =>
        Player.LocalInstance.OnSelectedCounterChanged -= OnPlayerAnyPlayerSpawn;

    private void SetVisualActive(bool active)
    {
        foreach (var visualGameObject in visualGameObjects)
        {
            visualGameObject.SetActive(active);
        }
    }
        
}
