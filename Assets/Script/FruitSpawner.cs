using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitSpawner : MonoBehaviour
{
    // loaded at Awake() from Assets/Resources/Prefabs/
    private GameObject[] fruitPrefabs;

    // Area in which fruits can randomly spawn around the spawner's position
    public Vector3 spawnArea = Vector3.zero;

    // Initial time between spawns (in seconds)
    public float spawnInterval = 7f;
    public float spawnIntervalDecrease = 0.1f;
    public float minSpawnInterval = 1f;

    // Initial speed range (toward the Player)
    public float minInitialSpeed = 2f;
    public float maxInitialSpeed = 10f;

    // Maximum angular drift speed
    public float maxRotationSpeed = 5f;

    // Maximum simultaneous fruits
    public int maxFruits = 30;

    // Tracks active fruits
    private List<GameObject> activeFruits = new List<GameObject>();

    void Awake()
    {
        fruitPrefabs = new GameObject[]
        {
            Resources.Load<GameObject>("Prefabs/Apple_LP"),
            Resources.Load<GameObject>("Prefabs/Banana_LP"),
            Resources.Load<GameObject>("Prefabs/Pear_LP"),
            Resources.Load<GameObject>("Prefabs/Pineapple_LP"),
            Resources.Load<GameObject>("Prefabs/Orange_LP")
        };
    }

    void Start()
    {
        StartCoroutine(SpawnFruits());
    }

    IEnumerator SpawnFruits()
    {
        while (true)
        {
            // wait before spawning next fruit
            yield return new WaitForSeconds(spawnInterval);

            // clean up any destroyed fruits
            activeFruits.RemoveAll(f => f == null);

            // spawn a new fruit
            int idx = UnityEngine.Random.Range(0, fruitPrefabs.Length);
            Vector3 spawnPos = transform.position + new Vector3(
                UnityEngine.Random.Range(-spawnArea.x, spawnArea.x),
                UnityEngine.Random.Range(-spawnArea.y, spawnArea.y),
                UnityEngine.Random.Range(-spawnArea.z, spawnArea.z)
            );
            GameObject fruit = Instantiate(fruitPrefabs[idx], spawnPos, Quaternion.identity);
            activeFruits.Add(fruit);

            // if we've exceeded the cap, destroy the fruit farthest from the player
            if (activeFruits.Count > maxFruits)
            {
                // find the farthest fruit
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    float maxDistSq = -1f;
                    GameObject farthest = null;
                    Vector3 pPos = player.transform.position;

                    foreach (var f in activeFruits)
                    {
                        float dSq = (f.transform.position - pPos).sqrMagnitude;
                        if (dSq > maxDistSq)
                        {
                            maxDistSq = dSq;
                            farthest = f;
                        }
                    }

                    if (farthest != null)
                    {
                        activeFruits.Remove(farthest);
                        Destroy(farthest);
                    }
                }
                else
                {
                    // fallback: just remove the first one
                    var oldest = activeFruits[0];
                    activeFruits.RemoveAt(0);
                    if (oldest != null)
                        Destroy(oldest);
                }
            }

            // physics setup
            Rigidbody rb = fruit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.linearDamping = 0f;
                rb.angularDamping = 0f;

                // direction toward the Player at spawn time
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Vector3 dir = (player != null)
                    ? (player.transform.position - spawnPos).normalized
                    : Vector3.zero;

                // random initial speed
                float speed = UnityEngine.Random.Range(minInitialSpeed, maxInitialSpeed);
                rb.linearVelocity = dir * speed;

                // random angular drift
                Vector3 axis = UnityEngine.Random.onUnitSphere;
                float rotSpeed = UnityEngine.Random.Range(0f, maxRotationSpeed);
                rb.angularVelocity = axis * rotSpeed;
            }

            // speed up next spawn
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecrease);
        }
    }
}
