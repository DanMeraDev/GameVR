using Oculus.Interaction;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class FlashLight : MonoBehaviour
{
    [Header("Linterna")]
    public Light linternaLuz;
    private Renderer render;
    public float batteryLife = 200f;
    public float batteryDrainRate = 2.2f;
    public float maxBattery = 300f;
    
    public GameObject meshConeLigth;
    public UnityEngine.Color emisionColor = UnityEngine.Color.white;
    private AudioSource onOffAudioSource;
    private bool luzEncendida = false;
    private Grabbable grabbable;


    //Sounds
    [Header("EffectSunds")]
    public AudioClip recarghSound;
    public AudioClip onOffSound;

    [Header("Input Reference")]
    public InputActionReference A_button;


    [Header("Battery Check")]
    public TextMeshProUGUI batteryStateText;

    //bool de Seleccion objeto
    private bool isGrabbed = false;
    void Awake()
    {
        render = GetComponent<Renderer>();
        grabbable = GetComponent<Grabbable>();
        onOffAudioSource = this.transform.AddComponent<AudioSource>();
        onOffAudioSource.clip = onOffSound;
        onOffAudioSource.loop = false;
        meshConeLigth.SetActive(false);
        batteryStateText.enabled = false;


    }
    private void OnEnable()
    {
        if (grabbable != null)
        {
            // Nos suscribimos a los eventos del grabbable
            grabbable.WhenPointerEventRaised += OnPointerEvent;
            A_button.action.performed += onPressButton;
        }

    }

    private void OnDisable()
    {
        if (grabbable != null)
        {
            // Nos suscribimos a los eventos del grabbable
            grabbable.WhenPointerEventRaised -= OnPointerEvent;
            A_button.action.performed -= onPressButton;
        }

    }

    private void onPressButton(InputAction.CallbackContext ctx)
    {
        Debug.Log("ON por botton");
        if(isGrabbed)
        {
            if (!luzEncendida)
            {
                EncenderLinterna();
            }
            else {
                ApagarLinterna();
            }
        }
    }
    void Start()
    {
        if (linternaLuz != null)
            linternaLuz.enabled = false;
    }
    private void OnPointerEvent(PointerEvent evt)
    {

        if (evt.Type == PointerEventType.Select)
        {
            isGrabbed = true;
            // 👉 Detecta si está agarrado con Oculus
            if (grabbable.SelectingPointsCount > 0) // está agarrado
            {
                //Desactiva el outliner del objeto 
                if (render.materials.Length > 1)
                    render.materials[1].SetFloat("_Outline_Thickness", 0f);
                if (!luzEncendida)
                    EncenderLinterna();
            }
                
            // vibración corta
            OVRInput.SetControllerVibration(1f, 0.5f, OVRInput.Controller.RTouch); // Right hand
            OVRInput.SetControllerVibration(1f, 0.5f, OVRInput.Controller.LTouch); // Left hand

            //Detener después de un tiempo corto
            Invoke(nameof(StopHaptics), 0.2f);
        }
        if (evt.Type == PointerEventType.Unselect)
        {
            isGrabbed = false;
            if (render.materials.Length > 1)
                render.materials[1].SetFloat("_Outline_Thickness", 0.002999999f);
            if (luzEncendida)
                ApagarLinterna();
        }

    }

    //Update de la linterna
    void Update()
    {
        logicaLinterna();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            RechargeBattery(75);
            other.gameObject.SetActive(false);
        }
    }

    //Secion de funciones
    private void logicaLinterna()
    {
        // 👉 Drenar batería si está encendida
        if (luzEncendida)
        {
            batteryLife -= batteryDrainRate * Time.deltaTime;
            if (batteryLife <= 0)
            {
                
                batteryLife = 0;
                ApagarLinterna();
            }
            batteryStateText.text = "Battery: " + batteryLife;
        }
    }
    private void EncenderLinterna()
    {
        if (batteryLife > 0)
        {
            batteryStateText.enabled = true;
            onOffAudioSource.Play();
            luzEncendida = true;
            linternaLuz.enabled = true;
            meshConeLigth.SetActive(true);
            
        }
    }

    private void ApagarLinterna()
    {
        batteryStateText.enabled = false;
        onOffAudioSource.Play();
        luzEncendida = false;
        meshConeLigth.SetActive(false);
        if (linternaLuz != null)
            linternaLuz.enabled = false;
    }

    public void RechargeBattery(float amount = 50f)
    {
        onOffAudioSource.PlayOneShot(recarghSound, 1f);
        batteryLife = Mathf.Clamp(batteryLife + amount, 0f, maxBattery);
        Debug.Log("Batería recargada. Nivel actual: " + batteryLife);
    }
    private void StopHaptics()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }
}
