using UnityEngine;
using System.Collections;

public class Crate_Explosion : MonoBehaviour
{
    [Header("Parts to Explode")]
    public GameObject[] parts;

    [Header("Explosion Settings")]
    [Range(0f, 100f)]
    public float explosionStrength = 30f;
    public float explosionRadius = 5f;
    public float upwardModifier = 0.4f;
    public float partLifetime = 3f;

    private bool hasExploded = false;

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        foreach (GameObject part in parts)
        {
            // Detach from parent (crate)
            part.transform.parent = null;

            // Add Rigidbody if not already there
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb == null)
                rb = part.AddComponent<Rigidbody>();

            // Apply explosion force
            rb.AddExplosionForce(explosionStrength, transform.position, explosionRadius, upwardModifier, ForceMode.Impulse);

            // Optionally random torque
            rb.AddTorque(Random.onUnitSphere * (explosionStrength * 0.5f), ForceMode.Impulse);

            // Start disappearing coroutine
            StartCoroutine(DestroyAfterDelay(part, partLifetime));
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }

    // Optional: Debug explode with a key
    void Update()
    {
    }
}