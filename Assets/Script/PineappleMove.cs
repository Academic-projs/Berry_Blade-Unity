using System.Collections;
using UnityEngine;

public class PineappleMove : MonoBehaviour
{
    // Reference to the player's transform (the cube)
    public Transform target;

    // Offset from the player's position (optional)
    public Vector3 offset = Vector3.zero;

    // How fast the pineapple catches up to the player
    public float chaseSpeed = 5f;

    // Force of the pushback after collision
    public float pushForce = 2f;

    // Time to pause after pushback
    public float pushbackPauseTime = 0.5f;

    // Time to keep the player red after collision
    public float redDuration = 0.5f;

    // Additional parameters for unique sine wave motion
    public float sineAmplitude = 1.0f;
    public float sineFrequency = 2.0f;

    // Internal variables to manage pushback state and color
    private bool isPushedBack = false;
    private float pushbackTimer = 0f;
    private Renderer targetRenderer;
    private Color originalColor;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        // Get the target's Renderer and store the original color
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
        // If in pushback pause, update timer and skip normal movement
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
        // 1) If it�s the player, do your pushback + red-flash logic:
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
        // Change the player's color to red
        targetRenderer.material.color = Color.red;
        yield return new WaitForSeconds(redDuration);
        // Revert the player's color to the original
        targetRenderer.material.color = originalColor;
    }
}
