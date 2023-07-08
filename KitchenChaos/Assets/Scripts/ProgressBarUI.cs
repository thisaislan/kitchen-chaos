using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CuttingCounter;

public class ProgressBarUI : MonoBehaviour
{

    [SerializeField] private CuttingCounter cuttingCounter;
    [SerializeField] private Image barImage;

    private void Start()
    {
        cuttingCounter.OnProgressChanged += CuttingCounterOnProgressChangedEvent;

        barImage.fillAmount = 0;

        SetActive(false);
    }

    private void OnDestroy()
    {
        cuttingCounter.OnProgressChanged -= CuttingCounterOnProgressChangedEvent;
    }

    private void CuttingCounterOnProgressChangedEvent(
            object sender,
            CuttingCounter.OnProgressChangedEventArgs onProgressChangedEventArgs
        )
    {
        barImage.fillAmount = onProgressChangedEventArgs.ProgressNormalized;

        SetActive(barImage.fillAmount != 0f && barImage.fillAmount != 1f);
    }

    private void SetActive(bool active) =>
         gameObject.SetActive(active);

}
