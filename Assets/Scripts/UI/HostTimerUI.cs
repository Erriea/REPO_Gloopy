using TMPro;
using UnityEngine;

public class HostTimerUI : MonoBehaviour
{
    [SerializeField] private PlayerStateController playerStateController;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float hostDuration = 10f;
    [SerializeField] private float airborneDropRayStartHeight = 10f;
    [SerializeField] private float airborneDropRayDistance = 30f;
    [SerializeField] private float baseGroundOffset = 0.15f;

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

        if (GameManager.Instance != null && GameManager.Instance.IsGameplayLocked())
        {
            RefreshVisualState();
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
        bool swimmerDrowned = activeHost != null &&
                              activeHost.HostType == HostType.Swimmer &&
                              activeHost.IsOverWater();

        if (swimmerDrowned)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver("You Drowned");
            }

            playerStateController.EnterDrowningGameOver();
            activeHost.Die();
            currentTimeRemaining = hostDuration;
            RefreshVisualState();
            return;
        }

        if (activeHost != null && activeHost.HostType == HostType.Flyer)
        {
            if (TryResolveAirborneDrop(activeHost, out Vector3 landingPosition, out bool drownedOnLanding))
            {
                if (drownedOnLanding)
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.TriggerGameOver("You Drowned");
                    }

                    playerStateController.EnterDrowningGameOver();
                    activeHost.Die();
                    currentTimeRemaining = hostDuration;
                    RefreshVisualState();
                    return;
                }

                playerStateController.ReturnToBaseForm(landingPosition);
                activeHost.Die();
                currentTimeRemaining = hostDuration;
                RefreshVisualState();
                return;
            }
        }

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

    private bool TryResolveAirborneDrop(HostController airborneHost, out Vector3 landingPosition, out bool drownedOnLanding)
    {
        Vector3 airbornePosition = airborneHost != null ? airborneHost.transform.position : playerStateController.transform.position;
        landingPosition = airbornePosition;
        drownedOnLanding = false;

        Vector3 rayStart = airbornePosition + Vector3.up * airborneDropRayStartHeight;
        RaycastHit[] hits = Physics.RaycastAll(
            rayStart,
            Vector3.down,
            airborneDropRayDistance,
            Physics.AllLayers,
            QueryTriggerInteraction.Collide);

        if (hits == null || hits.Length == 0)
        {
            return false;
        }

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        int waterLayer = LayerMask.NameToLayer("Water");

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            if (airborneHost != null && hit.collider.transform.IsChildOf(airborneHost.transform))
            {
                continue;
            }

            if (hit.collider.gameObject.layer == waterLayer)
            {
                drownedOnLanding = true;
                return true;
            }

            if (hit.collider.isTrigger)
            {
                continue;
            }

            landingPosition = hit.point + Vector3.up * baseGroundOffset;
            return true;
        }

        return false;
    }
}
