using UnityEngine;
using Oculus.Interaction;
using Unity.VisualScripting;
public class FlashLight : MonoBehaviour
{
    [Header("Linterna")]
    public Light linternaLuz;
    public float batteryLife = 200f;
    public float batteryDrainRate = 2.2f;
    public float maxBattery = 300f;
    public AudioClip onOffSound;
    public GameObject meshConeLigth;

    private AudioSource onOffAudioSource;
    private bool luzEncendida = false;
    private Grabbable grabbable;

    void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        onOffAudioSource = this.transform.AddComponent<AudioSource>();
        onOffAudioSource.clip = onOffSound;
        onOffAudioSource.loop = false;
        meshConeLigth.SetActive(false);
    }

    void Start()
    {
        if (linternaLuz != null)
            linternaLuz.enabled = false;
    }
    //Update de la linterna
    void Update()
    {
        logicaLinterna();
    }
    

    //Secion de funciones
    private void logicaLinterna()
    {
        // 👉 Detecta si está agarrado con Oculus
        if (grabbable.SelectingPointsCount > 0) // está agarrado
        {
            if (!luzEncendida)
                EncenderLinterna();
        }
        else
        {
            if (luzEncendida)
                ApagarLinterna();
        }

        // 👉 Drenar batería si está encendida
        if (luzEncendida)
        {
            batteryLife -= batteryDrainRate * Time.deltaTime;
            if (batteryLife <= 0)
            {
                batteryLife = 0;
                ApagarLinterna();
            }
        }
    }
    private void EncenderLinterna()
    {
        if (batteryLife > 0)
        {
            onOffAudioSource.Play();
            luzEncendida = true;
            linternaLuz.enabled = true;
            meshConeLigth.SetActive(true);
        }
    }

    private void ApagarLinterna()
    {
        onOffAudioSource.Play();
        luzEncendida = false;
        meshConeLigth.SetActive(false);
        if (linternaLuz != null)
            linternaLuz.enabled = false;
    }

    public void RechargeBattery(float amount = 50f)
    {
        batteryLife = Mathf.Clamp(batteryLife + amount, 0f, maxBattery);
        Debug.Log("Batería recargada. Nivel actual: " + batteryLife);
    }
}
