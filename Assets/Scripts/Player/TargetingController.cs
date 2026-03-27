using UnityEngine;

public class TargetingController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerStateController playerStateController;
    [SerializeField] private float infectionRange = 4f;
    [SerializeField] private Transform targetMarker;
    [SerializeField] private Vector3 targetMarkerOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private TooFarPopupUI tooFarPopupUI;

    private HostController selectedHost;

    public HostController SelectedHost => selectedHost;

    private void Update()
    {
        HandleSelectionInput();
        HandleInfectionInput();
        ClearSelectionIfOutOfRange();
        UpdateTargetMarker();
    }

    private void HandleSelectionInput()
    {
        if (!Input.GetMouseButtonDown(0) || mainCamera == null)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return;
        }

        HostController host = hit.collider.GetComponentInParent<HostController>();
        if (host == null || !host.CanBeInfected())
        {
            selectedHost = null;
            return;
        }

        if (!IsHostInRange(host))
        {
            selectedHost = null;
            ShowTooFarPopup();
            return;
        }

        selectedHost = host;
        AudioManager.Instance?.PlayTargetHost();
    }

    private void HandleInfectionInput()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || selectedHost == null || playerStateController == null)
        {
            return;
        }

        if (!IsHostInRange(selectedHost))
        {
            ShowTooFarPopup();
            return;
        }

        bool infectionSucceeded = playerStateController.InfectHost(selectedHost);
        if (infectionSucceeded)
        {
            selectedHost = null;
        }
    }

    private void ClearSelectionIfOutOfRange()
    {
        if (selectedHost == null)
        {
            return;
        }

        if (!selectedHost.CanBeInfected() || !IsHostInRange(selectedHost))
        {
            selectedHost = null;
        }
    }

    private bool IsHostInRange(HostController host)
    {
        Transform activeTransform = playerStateController != null ? playerStateController.GetActiveTransform() : null;
        if (host == null || activeTransform == null)
        {
            return false;
        }

        float distance = Vector3.Distance(activeTransform.position, host.transform.position);
        return distance <= infectionRange;
    }

    private void UpdateTargetMarker()
    {
        if (targetMarker == null)
        {
            return;
        }

        bool shouldShowMarker = selectedHost != null;
        targetMarker.gameObject.SetActive(shouldShowMarker);

        if (shouldShowMarker)
        {
            targetMarker.position = selectedHost.transform.position + targetMarkerOffset;
        }
    }

    private void ShowTooFarPopup()
    {
        if (tooFarPopupUI != null)
        {
            tooFarPopupUI.ShowMessage();
        }
    }
}
