using System.Collections.Generic;
using UnityEngine;

public class HostSpawnManager : MonoBehaviour
{
    [System.Serializable]
    private class HostSpawnGroup
    {
        public string label;
        public HostController hostPrefab;
        [Min(0)] public int spawnCount = 1;
        public bool allowSpawnPointReuse = true;
        [Min(0f)] public float horizontalSpawnRadius = 0f;
        public bool applySpawnPointRotation = false;
        public List<Transform> spawnPoints = new();
    }

    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool clearSpawnedHostsBeforeRespawn = true;
    [SerializeField] private Transform spawnedHostsParent;
    [SerializeField] private List<HostSpawnGroup> spawnGroups = new();

    private readonly List<GameObject> spawnedHosts = new();

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnAllHosts();
        }
    }

    public void SpawnAllHosts()
    {
        if (clearSpawnedHostsBeforeRespawn)
        {
            ClearSpawnedHosts();
        }

        foreach (HostSpawnGroup group in spawnGroups)
        {
            SpawnGroup(group);
        }
    }

    public void ClearSpawnedHosts()
    {
        for (int i = spawnedHosts.Count - 1; i >= 0; i--)
        {
            if (spawnedHosts[i] != null)
            {
                Destroy(spawnedHosts[i]);
            }
        }

        spawnedHosts.Clear();
    }

    private void SpawnGroup(HostSpawnGroup group)
    {
        if (group == null || group.hostPrefab == null || group.spawnCount <= 0)
        {
            return;
        }

        List<Transform> validSpawnPoints = new();
        foreach (Transform spawnPoint in group.spawnPoints)
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

        List<Transform> availableSpawnPoints = new(validSpawnPoints);
        int spawnTotal = group.allowSpawnPointReuse
            ? group.spawnCount
            : Mathf.Min(group.spawnCount, availableSpawnPoints.Count);

        for (int i = 0; i < spawnTotal; i++)
        {
            List<Transform> spawnPool = group.allowSpawnPointReuse ? validSpawnPoints : availableSpawnPoints;
            if (spawnPool.Count == 0)
            {
                break;
            }

            int randomIndex = Random.Range(0, spawnPool.Count);
            Transform chosenSpawnPoint = spawnPool[randomIndex];

            if (!group.allowSpawnPointReuse)
            {
                availableSpawnPoints.RemoveAt(randomIndex);
            }

            Vector3 spawnPosition = GetSpawnPosition(chosenSpawnPoint, group.horizontalSpawnRadius);

            Quaternion spawnRotation = group.hostPrefab.transform.rotation;
            if (group.applySpawnPointRotation)
            {
                spawnRotation = chosenSpawnPoint.rotation * spawnRotation;
            }

            HostController spawnedHost = Instantiate(
                group.hostPrefab,
                spawnPosition,
                spawnRotation,
                spawnedHostsParent);

            if (spawnedHost != null && !spawnedHost.IsValidSpawnLocation())
            {
                Destroy(spawnedHost.gameObject);
                continue;
            }

            spawnedHosts.Add(spawnedHost.gameObject);
        }
    }

    private Vector3 GetSpawnPosition(Transform spawnPoint, float horizontalSpawnRadius)
    {
        if (spawnPoint == null)
        {
            return Vector3.zero;
        }

        Vector3 position = spawnPoint.position;
        if (horizontalSpawnRadius <= 0f)
        {
            return position;
        }

        Vector2 randomOffset = Random.insideUnitCircle * horizontalSpawnRadius;
        return new Vector3(
            position.x + randomOffset.x,
            position.y,
            position.z + randomOffset.y);
    }
}
