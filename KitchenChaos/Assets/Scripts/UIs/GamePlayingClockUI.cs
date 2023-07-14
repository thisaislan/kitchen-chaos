using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{

    [SerializeField] private Image timerImage;

    private void Update() =>
        timerImage.fillAmount = GameManager.Instance.GetGamePlayTimeNormalized();

}
