using UnityEngine;

public class HostMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask waterLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private float terrainCheckRadius = 0.35f;
    [SerializeField] private float obstacleCheckDistance = 0.6f;

    private bool playerControlEnabled;
    private HostType hostType = HostType.Walker;

    private void Update()
    {
        if (!playerControlEnabled)
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        if (moveDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        TryMove(moveDirection, moveSpeed);
    }

    public void SetPlayerControlEnabled(bool enabled)
    {
        playerControlEnabled = enabled;
    }

    public void SetHostType(HostType newHostType)
    {
        hostType = newHostType;
    }

    public bool TryMove(Vector3 moveDirection, float speed)
    {
        if (moveDirection.sqrMagnitude <= 0f)
        {
            return false;
        }

        Vector3 normalizedDirection = moveDirection.normalized;
        Vector3 desiredPosition = transform.position + normalizedDirection * speed * Time.deltaTime;
        if (!CanMoveTo(desiredPosition, normalizedDirection))
        {
            return false;
        }

        transform.position = desiredPosition;
        return true;
    }

    private bool CanMoveTo(Vector3 desiredPosition, Vector3 moveDirection)
    {
        bool isOverWater = IsOverWater(desiredPosition);

        switch (hostType)
        {
            case HostType.Walker:
                if (isOverWater || IsObstacleAhead(moveDirection))
                {
                    return false;
                }

                return true;

            case HostType.Swimmer:
                return isOverWater;

            case HostType.Flyer:
                return true;

            default:
                return true;
        }
    }

    private bool IsOverWater(Vector3 worldPosition)
    {
        if (waterLayerMask.value == 0)
        {
            return false;
        }

        Vector3 checkPosition = worldPosition + Vector3.down * 0.5f;
        return Physics.CheckSphere(checkPosition, terrainCheckRadius, waterLayerMask, QueryTriggerInteraction.Collide);
    }

    private bool IsObstacleAhead(Vector3 moveDirection)
    {
        if (obstacleLayerMask.value == 0)
        {
            return false;
        }

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        return Physics.Raycast(origin, moveDirection, obstacleCheckDistance, obstacleLayerMask, QueryTriggerInteraction.Ignore);
    }
}
