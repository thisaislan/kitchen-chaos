using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{

    [SerializeField] private BaseCounter counter;
    [SerializeField] private GameObject[] visualGameObjects;

    void Start()
    {
       // Player.Instance.OnSelectedCounterChanged += OnPlayerSelectedCounterChanged;
    }

    private void OnDestroy()
    {
      //  Player.Instance.OnSelectedCounterChanged -= OnPlayerSelectedCounterChanged;
    }

    private void OnPlayerSelectedCounterChanged(object sender, Player.OnSelectedChangedEventArgs playerEventArgs) =>
        SetVisualActive(playerEventArgs.SelectedCounter == counter);

    private void SetVisualActive(bool active)
    {
        foreach (var visualGameObject in visualGameObjects)
        {
            visualGameObject.SetActive(active);
        }
    }
        

}
