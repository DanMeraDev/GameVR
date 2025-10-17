using Oculus.Haptics;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class GrapWithHapticSource : MonoBehaviour
{
    private GrabInteractable grabInteractable;
    [Header("Configuración de Vibración")]
    [Tooltip("Intensidad de la vibración (0 a 1).")]
    [Range(0f, 1f)]
    public float vibrationAmplitude = 0.5f;

    [Tooltip("Frecuencia de la vibración (0 a 1).")]
    [Range(0f, 1f)]
    public float vibrationFrequency = 1f;

    [Tooltip("Duración de la vibración en segundos.")]
    public float vibrationDuration = 0.1f;
    [Header("Configuración de Vibración")]
    public AudioClip vibrationClip;
    private AudioSource vibrationSource;

    private void Awake()
    {

        grabInteractable= GetComponent<GrabInteractable>();
        vibrationSource = this.transform.AddComponent<AudioSource>();

    }

    private void OnEnable()
    {
        grabInteractable.WhenSelectingInteractorAdded.Action += onInteraction;
      //  grabbable.WhenSelectingInteractorAdded += OnGrabbed;

    }

    private void OnDisable()
    {
        // grabbable.WhenPointerEventRaised -= OnGrabbed;
        grabInteractable.WhenSelectingInteractorAdded.Action -= onInteraction;

    }
    private void onInteraction(GrabInteractor interactor)
    {
        Debug.Log("Log intro");
        var interactorMono = interactor as MonoBehaviour;
        if (interactorMono == null) return;
        Debug.Log("Log intro2");
        // 1. Imprime el nombre del GameObject para saber cuál es.
        Debug.Log($"El nombre del interactor es: {interactorMono.gameObject.name}");

        // 2. Imprime el objeto mismo para poder hacerle clic en la consola y verlo en la jerarquía.
        Debug.Log("Haz clic aquí para seleccionar el objeto interactor:", interactorMono.gameObject);
        HandGrabInteractor handInteractor = interactorMono.GetComponent<HandGrabInteractor>();
        Debug.Log("Log handInteractor");
        Debug.Log(handInteractor);
        if (handInteractor != null)
        {
            Debug.Log("INtro Al vibrador");
            OVRInput.Controller controllerToVibrate = OVRInput.Controller.None;

            if (handInteractor.Hand.Handedness == Handedness.Left)
            {
                
                controllerToVibrate = OVRInput.Controller.LTouch;
            }
            else if (handInteractor.Hand.Handedness == Handedness.Right)
            {
                controllerToVibrate = OVRInput.Controller.RTouch;
            }

            if (controllerToVibrate != OVRInput.Controller.None)
            {
                Debug.Log("Vibrador");
                vibrationSource.PlayOneShot(vibrationClip, 1f);
                // Inicia la vibración en el control correcto
                OVRInput.SetControllerVibration(vibrationFrequency, vibrationAmplitude, controllerToVibrate);

                // Programa la detención de la vibración después de 'vibrationDuration' segundos
                Invoke(nameof(StopHaptics), vibrationDuration);
            }
        }
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

