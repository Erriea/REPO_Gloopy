using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WaterZone : MonoBehaviour
{
    private void Reset()
    {
        Collider zoneCollider = GetComponent<Collider>();
        zoneCollider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Water");
    }
}
