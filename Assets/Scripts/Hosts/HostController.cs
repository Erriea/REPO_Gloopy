using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class HostController : MonoBehaviour
{
    [SerializeField] private HostType hostType = HostType.Walker;
    [SerializeField] private HostMovementController movementController;
    [SerializeField] private HostWanderAI wanderAI;
    [SerializeField] private ParticleSystem deathParticlePrefab;
    [SerializeField] private float deathEffectDuration = 1f;
    [SerializeField] private Vector3 deathSquashScale = new Vector3(1.25f, 0.15f, 1.25f);

    private bool isInfected;
    private bool isDying;
    private Collider hostCollider;
    private Vector3 initialScale;

    public bool IsInfected => isInfected;
    public bool IsDying => isDying;
    public HostType HostType => hostType;

    private void Awake()
    {
        hostCollider = GetComponent<Collider>();
        initialScale = transform.localScale;

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
        return gameObject.activeInHierarchy && !isDying;
    }

    public bool IsOverWater()
    {
        return movementController != null && movementController.IsCurrentlyOverWater();
    }

    public void Die()
    {
        if (isDying)
        {
            return;
        }

        StartCoroutine(PlayDeathSequence());
    }

    private IEnumerator PlayDeathSequence()
    {
        isDying = true;
        isInfected = false;

        if (movementController != null)
        {
            movementController.SetPlayerControlEnabled(false);
        }

        if (wanderAI != null)
        {
            wanderAI.SetWanderEnabled(false);
        }

        if (hostCollider != null)
        {
            hostCollider.enabled = false;
        }

        SpawnDeathParticles();

        float elapsed = 0f;
        while (elapsed < deathEffectDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / deathEffectDuration);
            transform.localScale = Vector3.Lerp(initialScale, deathSquashScale, t);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void SpawnDeathParticles()
    {
        if (deathParticlePrefab == null)
        {
            return;
        }

        ParticleSystem spawnedParticles = Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
        spawnedParticles.Play();
        Destroy(spawnedParticles.gameObject, spawnedParticles.main.duration + spawnedParticles.main.startLifetime.constantMax);
    }
}
