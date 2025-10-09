using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

// source: https://www.youtube.com/watch?v=PMXD8MvbiQs

public class HandForce : MonoBehaviour
{
    public InputActionReference handForceIAR;
    public float handForceVal;
    public ParticleSystem handParticles;
    public Vector2 particleRange;
    public AudioSource audioSource;
    public Vector2 audioRange;

    private Rigidbody _playerRb;
    private float _triggerValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerRb = FindAnyObjectByType<XROrigin>().GetComponent<Rigidbody>();
    }

    
    private void OnEnable()
    {
        Debug.Log("OnEnable");
        handForceIAR.action.Enable();
        handForceIAR.action.performed += SetTriggerValue;
        handForceIAR.action.canceled += OnTriggerReleased;
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        handForceIAR.action.Disable();
        handForceIAR.action.performed -= SetTriggerValue;
        handForceIAR.action.canceled -= OnTriggerReleased;
    }

    private void SetTriggerValue(InputAction.CallbackContext context)
    {
        _triggerValue = context.ReadValue<float>();
        Debug.Log(_triggerValue);
    }
    
    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        _triggerValue = 0f; // Reset the force value to 0 when trigger is released
        Debug.Log("Trigger Released: " + _triggerValue);
    }
    
    // Update is called once per frame
    private void Update()
    {
        var main = handParticles.main;
        main.startLifetime = Mathf.Lerp(particleRange.x, particleRange.y, _triggerValue);
        audioSource.pitch = Mathf.Lerp(audioRange.x, audioRange.y, _triggerValue);
        
        if (_triggerValue > 0.01f)
        {
            if (!handParticles.isPlaying)
                handParticles.Play();
        }
        else
        {
            if (handParticles.isPlaying)
                handParticles.Stop();
        }
    }

    private void FixedUpdate()
    {
        _playerRb.AddForce(transform.forward * (_triggerValue * handForceVal * Time.fixedDeltaTime));
    }
    
}
