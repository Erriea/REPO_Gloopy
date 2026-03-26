using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private PlayerStateController playerStateController;
    [SerializeField] private float triggerRadiusPadding = 0.5f;
    [SerializeField] private bool randomizeSpawnOnStart = true;
    [SerializeField] private List<Transform> spawnPoints = new();

    private Collider goalCollider;

    private void Awake()
    {
        goalCollider = GetComponent<Collider>();
        goalCollider.isTrigger = true;
    }

    private void Start()
    {
        if (randomizeSpawnOnStart)
        {
            MoveToRandomSpawnPoint();
        }
    }

    private void Update()
    {
        if (playerStateController == null || GameManager.Instance == null || GameManager.Instance.IsGameplayLocked())
        {
            return;
        }

        Transform activeTransform = playerStateController.GetActiveTransform();
        if (activeTransform == null)
        {
            return;
        }

        Vector3 closestPoint = goalCollider.ClosestPoint(activeTransform.position);
        float distance = Vector3.Distance(activeTransform.position, closestPoint);

        if (distance <= triggerRadiusPadding)
        {
            GameManager.Instance.TriggerWin();
        }
    }

    public void MoveToRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            return;
        }

        List<Transform> validSpawnPoints = new();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                validSpawnPoints.Add(spawnPoint);
            }
        }

        if (validSpawnPoints.Count == 0)
        {
            return;
        }

        Transform chosenSpawnPoint = validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
        transform.SetPositionAndRotation(chosenSpawnPoint.position, chosenSpawnPoint.rotation);
    }
}
