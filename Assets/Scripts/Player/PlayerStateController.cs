using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public enum PlayerForm
    {
        BaseForm,
        HostForm
    }

    [SerializeField] private GameObject alienBaseObject;
    [SerializeField] private CameraFollow cameraFollow;

    private PlayerForm currentForm = PlayerForm.BaseForm;
    private HostController currentHost;

    public PlayerForm CurrentForm => currentForm;
    public HostController CurrentHost => currentHost;
    public event System.Action<HostController> HostEntered;
    public event System.Action ReturnedToBaseForm;

    private void Start()
    {
        ReturnToBaseForm(transform.position);
    }

    public bool CanInfect()
    {
        return GameManager.Instance == null || !GameManager.Instance.IsGameplayLocked();
    }

    public bool InfectHost(HostController targetHost)
    {
        if (!CanInfect() || targetHost == null || !targetHost.CanBeInfected())
        {
            return false;
        }

        if (currentHost == targetHost)
        {
            return false;
        }

        if (currentHost != null)
        {
            currentHost.Release();
        }

        currentHost = targetHost;
        currentHost.Infect();
        currentForm = PlayerForm.HostForm;

        if (alienBaseObject != null)
        {
            alienBaseObject.SetActive(false);
        }

        UpdateCameraTarget();
        HostEntered?.Invoke(currentHost);
        return true;
    }

    public void ReturnToBaseForm(Vector3 worldPosition)
    {
        if (currentHost != null)
        {
            currentHost.Release();
            currentHost = null;
        }

        currentForm = PlayerForm.BaseForm;
        transform.position = worldPosition;

        if (alienBaseObject != null)
        {
            alienBaseObject.SetActive(true);
            alienBaseObject.transform.position = worldPosition;
        }

        UpdateCameraTarget();
        ReturnedToBaseForm?.Invoke();
    }

    public void EnterDrowningGameOver()
    {
        if (currentHost != null)
        {
            currentHost.Release();
            currentHost = null;
        }

        currentForm = PlayerForm.BaseForm;

        if (alienBaseObject != null)
        {
            alienBaseObject.SetActive(false);
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(null);
        }

        ReturnedToBaseForm?.Invoke();
    }

    public Transform GetActiveTransform()
    {
        if (currentForm == PlayerForm.HostForm && currentHost != null)
        {
            return currentHost.transform;
        }

        return alienBaseObject != null ? alienBaseObject.transform : transform;
    }

    public Transform GetBaseFormTransform()
    {
        return alienBaseObject != null ? alienBaseObject.transform : transform;
    }

    public bool IsBaseFormExposed()
    {
        return currentForm == PlayerForm.BaseForm &&
               alienBaseObject != null &&
               alienBaseObject.activeInHierarchy;
    }

    private void UpdateCameraTarget()
    {
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(GetActiveTransform());
        }
    }
}
