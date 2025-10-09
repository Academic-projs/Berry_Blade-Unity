using UnityEngine;

public class ExplosionHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by explosion!");

           
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDir * 10f, ForceMode.Impulse);
            }
        }
    }
}
