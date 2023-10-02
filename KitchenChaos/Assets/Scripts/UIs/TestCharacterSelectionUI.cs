using UnityEngine;
using UnityEngine.UI;

public class TestCharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;

    private void Start() =>
        readyButton.onClick.AddListener(() => CharacterSelectReady.Instance.SetPlayerReady());

}
