using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class PearMove : MonoBehaviour
{
    // Reference to the player's transform
    public Transform target;

    // Optional offset from the player's position
    public Vector3 offset = Vector3.zero;

    // Speed at which the pear chases the player
    public float chaseSpeed = 5f;

    // Force of the pushback after collision
    public float pushForce = 2f;

    // Time to pause after pushback
    public float pushbackPauseTime = 0.5f;

    // Time to keep the player red after collision
    public float redDuration = 0.5f;

    // Parameters for bobbing motion (vertical oscillation)
    public float bobAmplitude = 0.5f;
    public float bobFrequency = 3f;

    private bool isPushedBack = false;
    private float pushbackTimer = 0f;

    private Renderer targetRenderer;
    private Color originalColor;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        // Cache the player's renderer and original color
        if (target != null)
        {

            targetRenderer = target.GetComponent<Renderer>();

            if (targetRenderer != null)
            {
                originalColor = targetRenderer.material.color;
            }
        }
    }

    void LateUpdate()
    {
        // During pushback, pause normal movement
        if (isPushedBack)
        {
            pushbackTimer += Time.deltaTime;
            if (pushbackTimer >= pushbackPauseTime)
            {
                isPushedBack = false;
            }
            return;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1) If it’s the player, do your pushback + red-flash logic:
        if (collision.transform == target)
        {
            Vector3 pushDir = (target.position - transform.position);
            pushDir.y = 0;
            pushDir.Normalize();

            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            if (targetRb != null)
                targetRb.AddForce(pushDir * pushForce, ForceMode.Impulse);

            transform.position -= pushDir * pushForce;

            isPushedBack = true;
            pushbackTimer = 0f;

            if (targetRenderer != null)
                StartCoroutine(TurnRedTemporarily());

            return;
        }

        // 2) Otherwise (walls, planets, anything else), just bounce:
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // reflect current velocity about the first contact normal
            Vector3 incoming = rb.linearVelocity;
            Vector3 normal = collision.contacts[0].normal;
            rb.linearVelocity = Vector3.Reflect(incoming, normal);
        }
    }


    private IEnumerator TurnRedTemporarily()
    {
        targetRenderer.material.color = Color.red;
        yield return new WaitForSeconds(redDuration);
        targetRenderer.material.color = originalColor;
    }
}
