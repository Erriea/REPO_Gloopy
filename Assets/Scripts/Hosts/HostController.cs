using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HostController : MonoBehaviour
{
    [SerializeField] private HostType hostType = HostType.Walker;
    [SerializeField] private HostMovementController movementController;
    [SerializeField] private HostWanderAI wanderAI;

    private bool isInfected;

    public bool IsInfected => isInfected;
    public HostType HostType => hostType;

    private void Awake()
    {
        if (movementController == null)
        {
            movementController = GetComponent<HostMovementController>();
        }

        if (wanderAI == null)
        {
            wanderAI = GetComponent<HostWanderAI>();
        }

        if (movementController != null)
        {
            movementController.SetHostType(hostType);
        }
    }

    public void Infect()
    {
        isInfected = true;

        if (movementController != null)
        {
            movementController.SetPlayerControlEnabled(true);
        }

        if (wanderAI != null)
        {
            wanderAI.SetWanderEnabled(false);
        }
    }

    public void Release()
    {
        isInfected = false;

        if (movementController != null)
        {
            movementController.SetPlayerControlEnabled(false);
        }

        if (wanderAI != null)
        {
            wanderAI.SetWanderEnabled(true);
        }
    }

    public bool CanBeInfected()
    {
        return gameObject.activeInHierarchy;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
