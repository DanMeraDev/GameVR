using Oculus.Haptics;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Grabbable))]
[RequireComponent(typeof(HapticSource))]
public class GrapWithHapticSource : MonoBehaviour
{
    private Grabbable grabbable;
    private HapticSource hapticSource;

    private void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        hapticSource = GetComponent<HapticSource>();
    }

    private void OnEnable()
    {
        grabbable.WhenPointerEventRaised += OnGrabbed;

    }

    private void OnDisable()
    {
        grabbable.WhenPointerEventRaised -= OnGrabbed;

    }

    private void OnGrabbed(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
        {

            // vibración corta
            OVRInput.SetControllerVibration(1f, 0.5f, OVRInput.Controller.RTouch); // Right hand
            OVRInput.SetControllerVibration(1f, 0.5f, OVRInput.Controller.LTouch); // Left hand

            //Detener después de un tiempo corto
            Invoke(nameof(StopHaptics), 0.2f);
        }
    }

    private void StopHaptics()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }

}

