using UnityEngine;
using System.Collections;

public class GrapeClusterMove : MonoBehaviour
{
    [Header("Target & Movement Settings")]
    public Transform target;          // The player to follow
    public float followSpeed = 5f;    // Speed of direct movement
    public float recoverySpeed = 3f;  // Speed to recover correct path after collision

    [Header("Bobbing Settings")]
    public float bobbingHeight = 0.2f;  // Bobbing amplitude
    public float bobbingSpeed = 2f;     // Bobbing speed

    [Header("Collision Effects")]
    public float hitPushback = 2f;      // Pushback force applied to player
    public float grapePushback = 1f;    // Force to push grapes back
    public float redDuration = 0.5f;    // How long the player stays red after being hit

    private bool isRecovering = false; // Flag to prevent multiple collisions
    private Rigidbody rb; // Rigidbody for the grape cluster
    private Vector3 lastValidTargetPos; // Stores last valid pathing direction

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure child grapes don’t have rigidbodies
        foreach (Transform child in transform)
        {
            Rigidbody childRb = child.GetComponent<Rigidbody>();
            if (childRb != null)
            {
                Destroy(childRb);  // Prevents physics conflicts with children
            }
        }

        if (target != null)
            lastValidTargetPos = target.position; // Initialize valid pathing position
    }

    void Update()
    {
        if (target == null) return;

        Vector3 direction;

        if (isRecovering)
        {
            // Smoothly transition back to the correct path after pushback
            direction = (lastValidTargetPos - transform.position).normalized;
            followSpeed = recoverySpeed; // Slow speed while recovering path
        }
        else
        {
            // Normal path-following behavior
            direction = (target.position - transform.position).normalized;
            lastValidTargetPos = target.position; // Continuously update valid path position
            followSpeed = 5f; // Restore full speed
        }

        // Apply bobbing effect
        float bobbingOffset = Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;

        // Compute new position with bobbing
        Vector3 newPosition = transform.position + (direction * followSpeed * Time.deltaTime);
        newPosition.y += bobbingOffset;

        // Move using Rigidbody (if available) for smooth movement
        if (rb != null)
            rb.MovePosition(newPosition);
        else
            transform.position = newPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == target && !isRecovering)
        {
            StartCoroutine(HitEffect(other.gameObject));
        }
    }

    IEnumerator HitEffect(GameObject player)
    {
        isRecovering = true;

        // Get player's Rigidbody
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null) yield break;

        // Get player's Renderer to change color
        Renderer playerRenderer = player.GetComponent<Renderer>();
        Color originalColor = Color.white;
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
            playerRenderer.material.color = Color.red;
        }

        // Pushback effect using AddForce
        Vector3 pushDir = (player.transform.position - transform.position).normalized;
        playerRb.AddForce(pushDir * hitPushback, ForceMode.Impulse);  // Apply push force to player
        if (rb != null)
        {
            rb.AddForce(-pushDir * grapePushback, ForceMode.Impulse); // Push the grapes back slightly
        }

        // Wait while the player stays red
        yield return new WaitForSeconds(redDuration);

        // Reset player's color
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
        }

        // Allow some time to recover before normal pursuit
        yield return new WaitForSeconds(0.3f);

        isRecovering = false;
    }
}
