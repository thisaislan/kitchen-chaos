using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour
{

    private const string IS_FLASHNG = "IsFlashing";

    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private Animator animator;

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

    private void OnStoveCounterTurnOff(object sender, EventArgs e)
    {
        animator.SetBool(IS_FLASHNG, false);
    }

    private void OnStoveCounterProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        var limitToShow = 0.5f;

        animator.SetBool(IS_FLASHNG, e.ProgressNormalized >= limitToShow && stoveCounter.IsBurning);
    }

}
