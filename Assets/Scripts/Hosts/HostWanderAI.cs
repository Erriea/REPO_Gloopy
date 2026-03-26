using UnityEngine;

public class HostWanderAI : MonoBehaviour
{
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float directionChangeInterval = 2.5f;
    [SerializeField] private float wanderRadius = 4f;

    private Vector3 spawnPosition;
    private Vector3 currentDirection;
    private float directionTimer;
    private bool wanderEnabled = true;
    private HostMovementController movementController;

    private void Awake()
    {
        spawnPosition = transform.position;
        movementController = GetComponent<HostMovementController>();
        PickNewDirection();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameplayLocked())
        {
            return;
        }

        if (!wanderEnabled)
        {
            return;
        }

        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f || Vector3.Distance(transform.position, spawnPosition) > wanderRadius)
        {
            PickNewDirection();
        }

        bool moved = movementController == null
            ? false
            : movementController.TryMove(currentDirection, wanderSpeed);

        if (!moved)
        {
            PickNewDirection();
        }
    }

    public void SetWanderEnabled(bool enabled)
    {
        wanderEnabled = enabled;

        if (wanderEnabled)
        {
            PickNewDirection();
        }
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
}
