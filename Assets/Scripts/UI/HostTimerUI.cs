using TMPro;
using UnityEngine;

public class HostTimerUI : MonoBehaviour
{
    [SerializeField] private PlayerStateController playerStateController;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float hostDuration = 10f;

    private float currentTimeRemaining;

    private void Start()
    {
        currentTimeRemaining = hostDuration;

        if (playerStateController != null)
        {
            playerStateController.HostEntered += HandleHostEntered;
            playerStateController.ReturnedToBaseForm += HandleReturnedToBaseForm;
        }

        RefreshVisualState();
    }

    private void OnDestroy()
    {
        if (playerStateController != null)
        {
            playerStateController.HostEntered -= HandleHostEntered;
            playerStateController.ReturnedToBaseForm -= HandleReturnedToBaseForm;
        }
    }

    private void Update()
    {
        if (playerStateController == null || timerText == null)
        {
            return;
        }

        if (playerStateController.CurrentForm != PlayerStateController.PlayerForm.HostForm)
        {
            RefreshVisualState();
            return;
        }

        currentTimeRemaining -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(Mathf.Max(currentTimeRemaining, 0f)).ToString();
        timerText.gameObject.SetActive(true);

        if (currentTimeRemaining > 0f)
        {
            return;
        }

        HostController activeHost = playerStateController.CurrentHost;
        Vector3 fallbackPosition = activeHost != null ? activeHost.transform.position : playerStateController.transform.position;

        playerStateController.ReturnToBaseForm(fallbackPosition);

        if (activeHost != null)
        {
            activeHost.Die();
        }

        currentTimeRemaining = hostDuration;
        RefreshVisualState();
    }

    private void HandleHostEntered(HostController host)
    {
        currentTimeRemaining = hostDuration;
        RefreshVisualState();
    }

    private void HandleReturnedToBaseForm()
    {
        currentTimeRemaining = hostDuration;
        RefreshVisualState();
    }

    private void RefreshVisualState()
    {
        bool showTimer = playerStateController != null &&
                         playerStateController.CurrentForm == PlayerStateController.PlayerForm.HostForm;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(showTimer);

            if (showTimer)
            {
                timerText.text = Mathf.CeilToInt(currentTimeRemaining).ToString();
            }
        }
    }
}
