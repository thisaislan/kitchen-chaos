using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{

    [SerializeField] private ClearCounter clearCounter;
    [SerializeField] private GameObject visualGameObject;

    void Start()
    {
        Player.Instance.OnSelectedCounterChanged += OnPlayerSelectedCounterChanged;
    }

    private void OnPlayerSelectedCounterChanged(object sender, Player.OnSelectedChangedEventArgs playerEventArgs) =>
        SetVisualActive(playerEventArgs.SelectedClearCounter == clearCounter);

    private void SetVisualActive(bool active) =>
        visualGameObject.SetActive(active);

}
