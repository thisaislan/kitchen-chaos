using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CuttingCounter;

public class ProgressBarUI : MonoBehaviour
{
        
    [SerializeField] GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;

    private IHasProgress hasProgress;

    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();

        hasProgress.OnProgressChanged += OnHasProgressChangedEvent;

        barImage.fillAmount = 0;

        SetActive(false);
    }

    private void OnDestroy()
    {
        hasProgress.OnProgressChanged -= OnHasProgressChangedEvent;
    }

    private void OnHasProgressChangedEvent(
            object sender,
            IHasProgress.OnProgressChangedEventArgs onProgressChangedEventArgs
        )
    {
        barImage.fillAmount = onProgressChangedEventArgs.ProgressNormalized;

        SetActive(barImage.fillAmount != 0f && barImage.fillAmount != 1f);
    }

    private void SetActive(bool active) =>
         gameObject.SetActive(active);

}
