using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoveBurnWarningUI : MonoBehaviour
{

    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private Image wariningImage;

    private float intervalToPlaySound = 0.2f;

    private void Start()
    {
        stoveCounter.OnProgressChanged += OnStoveCounterProgressChanged;
        stoveCounter.OnStoveTurnOff += OnStoveCounterTurnOff; 
    }

    private void OnDestroy()
    {
        stoveCounter.OnProgressChanged -= OnStoveCounterProgressChanged;
        stoveCounter.OnStoveTurnOff -= OnStoveCounterTurnOff;
    }

    private void OnStoveCounterTurnOff(object sender, System.EventArgs e)
    {
        HideWarning();
        StopAllCoroutines();
    }

    private void OnStoveCounterProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        var limitToShow = 0.5f;

        if (e.ProgressNormalized >= limitToShow && stoveCounter.IsBurning && !wariningImage.IsActive())
        {
            ShowWarning();
            StartCoroutine(StartPlaySound());
        }
    }

    private void ShowWarning() =>
        wariningImage.gameObject.SetActive(true);

    private void HideWarning() =>
        wariningImage.gameObject.SetActive(false);

    private IEnumerator StartPlaySound()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalToPlaySound);

            SoundManager.Instance.PlayWarningSound(this.transform.position);
        }

    }

}
