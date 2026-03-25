using TMPro;
using UnityEngine;

public class TooFarPopupUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private string message = "Target too far";
    [SerializeField] private float visibleDuration = 1f;
    [SerializeField] private float fadeDuration = 0.35f;

    private float timeRemaining;
    private CanvasGroup canvasGroup;
    private bool isShowing;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        HideImmediately();
    }

    private void Update()
    {
        if (!isShowing)
        {
            return;
        }

        if (timeRemaining <= 0f)
        {
            if (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.deltaTime / fadeDuration);
            }

            if (canvasGroup.alpha <= 0f)
            {
                HideImmediately();
            }

            return;
        }

        timeRemaining -= Time.deltaTime;
        canvasGroup.alpha = 1f;
    }

    public void ShowMessage()
    {
        isShowing = true;
        timeRemaining = visibleDuration;
        SetVisualState(1f, true);
    }

    private void HideImmediately()
    {
        isShowing = false;
        timeRemaining = 0f;
        SetVisualState(0f, false);
    }

    private void SetVisualState(float alpha, bool visible)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(visible);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }
}
