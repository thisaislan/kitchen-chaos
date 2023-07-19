using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{

    private const string POPUP_TRIGGER = "PopupTrigger";

    [SerializeField] private GameObject deliveryResultCounter;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;

    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;

    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;

    [SerializeField] private Animator animator;

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += OnDeliveryManagerRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += OnDeliveryManagerRecipeFailed;
    }

    private void OnDestroy()
    {
        DeliveryManager.Instance.OnRecipeSuccess -= OnDeliveryManagerRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed -= OnDeliveryManagerRecipeFailed;
    }

    private void OnDeliveryManagerRecipeFailed(object sender, System.EventArgs e)
    {
        deliveryResultCounter.SetActive(true);

        background.color = failedColor;
        icon.sprite = failedSprite;
        text.text = "DELIVERY\nFALIED!";

        animator.SetTrigger(POPUP_TRIGGER);
    }

    private void OnDeliveryManagerRecipeSuccess(object sender, System.EventArgs e)
    {
        deliveryResultCounter.SetActive(true);

        background.color = successColor;
        icon.sprite = successSprite;
        text.text = "DELIVERY\nSUCCESS!";

        animator.SetTrigger(POPUP_TRIGGER);
    }

}
