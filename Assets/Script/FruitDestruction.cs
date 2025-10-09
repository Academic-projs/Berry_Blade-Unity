using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class FruitDestruction : MonoBehaviour
{
    // Delay before destroying the fruit to allow the effect and sound to play.
    public float destroyDelay = 0.5f;

    // Squish sound to play upon collision.
    public AudioClip squishSound;

    // Reference to a smoke effect prefab
    public GameObject smokeEffectPrefab;

    [Header("Haptics")]
    [Range(0f, 1f)]
    public float hapticAmplitude = 0.5f;   // strength of the rumble
    public float hapticDuration = 0.1f;   // seconds

    // Internal AudioSource reference.
    private AudioSource audioSource;

    // Flag to prevent multiple triggers for the same fruit.
    private bool hasBeenHit = false;

    void Start()
    {
        // Try to get an AudioSource component; if none exists, add one.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Use OnCollisionEnter for non-trigger colliders.
    void OnCollisionEnter(Collision collision)
    {
        // Ensure we only process the collision once.
        if (hasBeenHit)
            return;

        // Change this tag as needed; here we expect the sword to have the tag "Sword".
        if (collision.gameObject.CompareTag("Sword"))
        {
            hasBeenHit = true;

            // --- HAPTICS ---
            var grabInteractable = collision.gameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null)
            {
                // Copy out the current interactorsSelecting into a list
                var selectors = new List<UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor>(grabInteractable.interactorsSelecting);
                if (selectors.Count > 0)
                {
                    var controllerInteractor = selectors[0] as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
                    if (controllerInteractor != null && controllerInteractor.xrController != null)
                    {
                        controllerInteractor.xrController
                            .SendHapticImpulse(hapticAmplitude, hapticDuration);
                    }
                }
            }

            // Play the squish sound.
            if (squishSound != null)
            {
                audioSource.PlayOneShot(squishSound);
            }

            // Instantiate the smoke effect at the fruit's position.
            if (smokeEffectPrefab != null)
            {
                Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity);
            }

            // Hide the fruit immediately so it appears to vanish.
            Renderer rend = GetComponent<Renderer>();
            if (rend != null)
            {
                rend.enabled = false;
            }
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            // Destroy the fruit object after a delay.
            Destroy(gameObject, destroyDelay);
        }
    }
}
