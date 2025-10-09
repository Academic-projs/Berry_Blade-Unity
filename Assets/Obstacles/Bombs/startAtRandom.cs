using UnityEngine;

public class startAtRandom : MonoBehaviour
{
    public GameObject[] myObjects;
    public Transform playerTransform;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int randomIndex = Random.Range(0, myObjects.Length);

            // Random spawn near the player
            Vector3 spawnPosition = playerTransform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
            GameObject explosion = Instantiate(myObjects[randomIndex], spawnPosition, Quaternion.identity);

            Rigidbody rb = explosion.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * 15f, ForceMode.Impulse);
            }
            Destroy(explosion, 2f); 
        }
        

    }
}
