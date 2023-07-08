using System;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{ 

    [SerializeField] private StoveCounter stoveCounter;

    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start()
    {
        stoveCounter.OnStoveTurnOn += OnStoveCounterTurnOn;
        stoveCounter.OnStoveTurnOff += OnStoveCounterTurnOff;
    }

    private void OnDestroy()
    {
        stoveCounter.OnStoveTurnOn -= OnStoveCounterTurnOn;
        stoveCounter.OnStoveTurnOff -= OnStoveCounterTurnOff;
    }

    private void OnStoveCounterTurnOn(object sender, System.EventArgs e)
    {
        stoveOnGameObject.SetActive(true);
        particlesGameObject.SetActive(true);
    }

    private void OnStoveCounterTurnOff(object sender, System.EventArgs e)
    {
        stoveOnGameObject.SetActive(false);
        particlesGameObject.SetActive(false);
    }
}
