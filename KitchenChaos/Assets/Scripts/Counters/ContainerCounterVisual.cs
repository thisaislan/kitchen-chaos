using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{

    private const string OPEN_CLOSE = "OpenClose";

    [SerializeField] private ContainerCounter containerCounter;

    [SerializeField] private Animator animator;

    private void Start()
    {
        containerCounter.OnPlayerGrabbedObject += OnPlayerGrabbedObject;
    }

    private void OnDestroy()
    {
        containerCounter.OnPlayerGrabbedObject -= OnPlayerGrabbedObject;
    }

    private void OnPlayerGrabbedObject(object sender, System.EventArgs eventArgs) =>
        animator.SetTrigger(OPEN_CLOSE);

}
