using UnityEngine;

public class HostMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Vector3 rotationOffsetEuler;
    [SerializeField] private float movementPulseFrequency = 8f;
    [SerializeField] private float movementStretchAmount = 0.08f;
    [SerializeField] private float scaleReturnSpeed = 8f;
    [SerializeField] private LayerMask waterLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private float terrainCheckRadius = 0.6f;
    [SerializeField] private float obstacleCheckDistance = 0.8f;
    [SerializeField] private float obstacleCheckRadius = 0.35f;
    [SerializeField] private float flyerHoverHeight = 2.5f;
    [SerializeField] private float flyerHeightSmoothing = 8f;
    [SerializeField] private float flyerBobAmplitude = 0.25f;
    [SerializeField] private float flyerBobFrequency = 4f;

    private bool playerControlEnabled;
    private HostType hostType = HostType.Walker;
    private Collider cachedCollider;
    private HostController hostController;
    private float flyerTargetHeight;
    private Quaternion initialRotationOffset;
    private Vector3 baseScale;
    private float movementPulseTimer;
    private float flyerBobTimer;
    private bool movedThisFrame;

    private void Awake()
    {
        cachedCollider = GetComponent<Collider>();
        hostController = GetComponent<HostController>();
        flyerTargetHeight = transform.position.y;
        initialRotationOffset = transform.rotation;
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameplayLocked())
        {
            return;
        }

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

    private void LateUpdate()
    {
        MaintainSpecialMovementState();
        UpdateMovementPulse();
        movedThisFrame = false;
    }

    public void SetPlayerControlEnabled(bool enabled)
    {
        playerControlEnabled = enabled;
    }

    public void SetHostType(HostType newHostType)
    {
        hostType = newHostType;

        if (hostType == HostType.Flyer)
        {
            flyerTargetHeight = flyerHoverHeight;
            SnapToFlyerHeight();
        }
    }

    public bool IsCurrentlyOverWater()
    {
        return IsOverWater(transform.position);
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

        if (hostType == HostType.Flyer)
        {
            // Preserve the current wave height while moving horizontally.
            desiredPosition.y = transform.position.y;
        }

        transform.position = desiredPosition;
        FaceMovementDirection(normalizedDirection);
        movedThisFrame = true;
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

        Vector3 origin = GetObstacleCheckOrigin();
        float castRadius = GetObstacleCheckRadius();
        return Physics.SphereCast(origin, castRadius, moveDirection, out _, obstacleCheckDistance, obstacleLayerMask, QueryTriggerInteraction.Ignore);
    }

    private Vector3 GetObstacleCheckOrigin()
    {
        if (cachedCollider == null)
        {
            return transform.position + Vector3.up * 0.5f;
        }

        Bounds bounds = cachedCollider.bounds;
        return new Vector3(bounds.center.x, bounds.center.y, bounds.center.z);
    }

    private float GetObstacleCheckRadius()
    {
        if (cachedCollider == null)
        {
            return obstacleCheckRadius;
        }

        Bounds bounds = cachedCollider.bounds;
        float derivedRadius = Mathf.Min(bounds.extents.x, bounds.extents.z) * 0.8f;
        return Mathf.Max(obstacleCheckRadius, derivedRadius);
    }

    private void MaintainSpecialMovementState()
    {
        if (hostType != HostType.Flyer)
        {
            return;
        }

        flyerBobTimer += Time.deltaTime * flyerBobFrequency;

        float bobOffset = Mathf.Sin(flyerBobTimer) * flyerBobAmplitude;
        Vector3 position = transform.position;
        position.y = Mathf.Lerp(position.y, flyerTargetHeight + bobOffset, flyerHeightSmoothing * Time.deltaTime);
        transform.position = position;
    }

    private void SnapToFlyerHeight()
    {
        Vector3 position = transform.position;
        position.y = flyerTargetHeight;
        transform.position = position;
        flyerBobTimer = 0f;
    }

    private void FaceMovementDirection(Vector3 moveDirection)
    {
        Vector3 flatDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
        if (flatDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(flatDirection.normalized) * Quaternion.Euler(rotationOffsetEuler);
        targetRotation *= initialRotationOffset;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void UpdateMovementPulse()
    {
        if (hostController != null && hostController.IsDying)
        {
            return;
        }

        if (hostType == HostType.Flyer)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, baseScale, scaleReturnSpeed * Time.deltaTime);
            return;
        }

        if (movedThisFrame)
        {
            movementPulseTimer += Time.deltaTime * movementPulseFrequency;
            float pulse = Mathf.Sin(movementPulseTimer) * movementStretchAmount;
            Vector3 pulsedScale = new Vector3(
                baseScale.x * (1f + pulse),
                baseScale.y * (1f - pulse),
                baseScale.z * (1f + pulse));

            transform.localScale = Vector3.Lerp(transform.localScale, pulsedScale, scaleReturnSpeed * Time.deltaTime);
            return;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, baseScale, scaleReturnSpeed * Time.deltaTime);
    }
}
