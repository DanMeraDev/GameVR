using UnityEngine;
using Oculus.Interaction;

public class GrabSoundGrabbable : MonoBehaviour
{
    private WrathGameManager gameManager;
    private Grabbable grabbable;

    void Start()
    {
        gameManager = FindFirstObjectByType<WrathGameManager>();
        grabbable = GetComponent<Grabbable>();

        if (grabbable != null)
        {
            // Nos suscribimos a los eventos del grabbable
            grabbable.WhenPointerEventRaised += OnPointerEvent;
        }
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
        {
            //sonido
            if (gameManager != null)
                gameManager.PlayGrabSound();

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

