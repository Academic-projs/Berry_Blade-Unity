using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class SwordOrientationTransformer : MonoBehaviour
{
    [Tooltip("How much to rotate the sword (in local Euler) when grabbed.")]
    public Vector3 rotationOffsetEuler = new Vector3(90f, 0f, 0f);

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grabInteractable;

    void Awake()
    {
        _grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void OnEnable()
    {
        _grabInteractable.selectEntered.AddListener(OnGrab);
    }

    void OnDisable()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Force the sword�s world rotation to:
        //    the attachTransform�s world rotation, plus our offset
        transform.rotation =
            _grabInteractable.attachTransform.rotation
            * Quaternion.Euler(rotationOffsetEuler);
    }
}
