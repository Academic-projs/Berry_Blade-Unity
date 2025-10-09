using UnityEngine;    // ← this is required for Collision, Collider, MonoBehaviour, Physics, etc.

public class SwordControl : MonoBehaviour
{
    private Collider swordCollider;
    private Collider playerCollider;

    void Start()
    {
        // cache your sword’s own collider
        swordCollider = GetComponent<Collider>();

        // find the player and its collider
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider>();

            // tell Unity to never collide sword ↔ player
            if (swordCollider != null && playerCollider != null)
                Physics.IgnoreCollision(swordCollider, playerCollider, true);
        }
    }

    // this method needs the Collision type from UnityEngine
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Fruit"))
        {
            Destroy(collision.gameObject);
        }
    }
}
