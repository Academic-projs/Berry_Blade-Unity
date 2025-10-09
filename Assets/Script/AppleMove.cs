using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Reference to the player's transform (the cube)
    public Transform target;

    // Offset from the player's position (optional)
    public Vector3 offset = new Vector3(0, 0, 0);

    // How fast the follower catches up to the player
    public float followSpeed = 5f;

    // Force of the pushback after collision
    public float pushForce = 2f;

    // Time to pause after pushback and turn red
    public float pushbackPauseTime = 0.5f;

    // Time to keep the target red after collision
    public float redDuration = 0.5f;

    private bool isPushedBack = false; // Tracks whether pushback is happening
    private float pushbackTimer = 0f;  // Timer to track the pause time

    private Renderer targetRenderer;  // Reference to the target's Renderer
    private Color originalColor;      // To store the target's original color

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
        if (isPushedBack)
        {
            // Handle the pause after pushback
            pushbackTimer += Time.deltaTime;
            if (pushbackTimer >= pushbackPauseTime)
            {
                isPushedBack = false;   // Resume normal movement after the pause
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


    private System.Collections.IEnumerator TurnRedTemporarily()
    {
        targetRenderer.material.color = Color.red;  // Turn the target red
        yield return new WaitForSeconds(redDuration);  // Wait for the specified duration
        targetRenderer.material.color = originalColor;  // Revert to the original color
    }
}
