using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{

    private const string CUT = "Cut";

    [SerializeField] private CuttingCounter cuttingCounter;

    [SerializeField] private Animator animator;

    private void Start()
    {
        cuttingCounter.OnCut += OnCuttingCounterCuts;
    }

    private void OnDestroy()
    {
        cuttingCounter.OnCut -= OnCuttingCounterCuts;
    }

    private void OnCuttingCounterCuts(object sender, System.EventArgs eventArgs) =>
        animator.SetTrigger(CUT);

}
