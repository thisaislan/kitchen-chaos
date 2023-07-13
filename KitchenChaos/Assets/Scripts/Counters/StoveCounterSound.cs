using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private StoveCounter stoveCounter;

    private void Start()
    {
        stoveCounter.OnStoveTurnOn += OnStoveCounterTurnOn;
        stoveCounter.OnStoveTurnOff += OnStoveCounterTurnOff;
    }

    private void OnStoveCounterTurnOff(object sender, System.EventArgs e) =>
        audioSource.Pause();

    private void OnStoveCounterTurnOn(object sender, System.EventArgs e) =>
        audioSource.Play();

}
