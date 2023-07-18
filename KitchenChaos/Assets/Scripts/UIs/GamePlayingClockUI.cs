using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{

    [SerializeField] private Image timerImage;

    private void Update()
    {
        if (GameManager.Instance.State == GameManager.GameState.GamePlaying)
        {
            timerImage.fillAmount = GameManager.Instance.GetGamePlayTimeNormalized();
        }
    }

}
