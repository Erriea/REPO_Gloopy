using UnityEngine;

public class PoliceAI : MonoBehaviour
{
    [SerializeField] private PlayerStateController playerStateController;
    [SerializeField] private float movementSpeed = 3.5f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Vector3 rotationOffsetEuler;
    [SerializeField] private float movementPulseFrequency = 8f;
    [SerializeField] private float movementStretchAmount = 0.06f;
    [SerializeField] private float scaleReturnSpeed = 8f;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float arrestDistance = 1.1f;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float directionChangeInterval = 2f;
    [SerializeField] private LayerMask waterLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private float terrainCheckRadius = 0.6f;
    [SerializeField] private float obstacleCheckDistance = 0.8f;
    [SerializeField] private float obstacleCheckRadius = 0.35f;

    private Vector3 spawnPosition;
    private Vector3 currentDirection;
    private float directionTimer;
    private Collider cachedCollider;
    private Vector3 baseScale;
    private float movementPulseTimer;
    private bool movedThisFrame;

    private void Awake()
    {
        spawnPosition = transform.position;
        cachedCollider = GetComponent<Collider>();
        baseScale = transform.localScale;
        PickNewDirection();
    }

    private void Update()
    {
        movedThisFrame = false;

        if (GameManager.Instance != null && GameManager.Instance.IsGameplayLocked())
        {
            UpdateMovementPulse();
            return;
        }

        if (playerStateController != null && playerStateController.IsBaseFormExposed())
        {
            Transform baseForm = playerStateController.GetBaseFormTransform();
            Vector3 toBase = baseForm.position - transform.position;
            float distanceToBase = toBase.magnitude;

            if (distanceToBase <= detectionRange)
            {
                Vector3 chaseDirection = new Vector3(toBase.x, 0f, toBase.z).normalized;
                TryMove(chaseDirection);
                FaceMovementDirection(chaseDirection);

                if (distanceToBase <= arrestDistance)
                {
                    GameManager.Instance?.TriggerGameOver("You Were Arrested");
                }

                UpdateMovementPulse();
                return;
            }
        }

        UpdateWander();
        UpdateMovementPulse();
    }

    private void UpdateWander()
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f || Vector3.Distance(transform.position, spawnPosition) > wanderRadius)
        {
            PickNewDirection();
        }

        if (!TryMove(currentDirection))
        {
            PickNewDirection();
            return;
        }

        FaceMovementDirection(currentDirection);
    }

    private bool TryMove(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude <= 0f)
        {
            return false;
        }

        Vector3 normalizedDirection = moveDirection.normalized;
        Vector3 desiredPosition = transform.position + normalizedDirection * movementSpeed * Time.deltaTime;
        if (IsOverWater(desiredPosition) || IsObstacleAhead(normalizedDirection))
        {
            return false;
        }

        transform.position = desiredPosition;
        movedThisFrame = true;
        return true;
    }

    private void PickNewDirection()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        currentDirection = new Vector3(randomDirection.x, 0f, randomDirection.y);
        directionTimer = directionChangeInterval;

        Vector3 offsetFromSpawn = transform.position - spawnPosition;
        if (offsetFromSpawn.sqrMagnitude > wanderRadius * wanderRadius)
        {
            currentDirection = new Vector3(-offsetFromSpawn.x, 0f, -offsetFromSpawn.z).normalized;
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
        return bounds.center;
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

    private void FaceMovementDirection(Vector3 moveDirection)
    {
        Vector3 flatDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
        if (flatDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(flatDirection.normalized) * Quaternion.Euler(rotationOffsetEuler);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void UpdateMovementPulse()
    {
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
