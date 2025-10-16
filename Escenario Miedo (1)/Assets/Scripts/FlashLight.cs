using Oculus.Interaction;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [Header("Linterna")]
    public Light linternaLuz;
    private Renderer render;
    public float batteryLife = 200f;
    public float batteryDrainRate = 2.2f;
    public float maxBattery = 300f;
    public AudioClip onOffSound;
    public GameObject meshConeLigth;
    public UnityEngine.Color emisionColor = UnityEngine.Color.white;
    private AudioSource onOffAudioSource;
    private bool luzEncendida = false;
    private Grabbable grabbable;

    void Awake()
    {
        render = GetComponent<Renderer>();
        grabbable = GetComponent<Grabbable>();
        onOffAudioSource = this.transform.AddComponent<AudioSource>();
        onOffAudioSource.clip = onOffSound;
        onOffAudioSource.loop = false;
        meshConeLigth.SetActive(false);
        render.materials[0].SetColor("_EmissionColor", emisionColor * 1f);

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
            // Desactiva la emisión de color blanco 
            render.materials[0].DisableKeyword("_EMISSION");
            //Desactiva el outliner del objeto 
            if (render.materials.Length > 1)
                render.materials[1].SetFloat("_Outline_Thickness", 0f);
            if (!luzEncendida)
                EncenderLinterna();
        }
        else
        {
            render.materials[0].DisableKeyword("_EMISSION");
            if (render.materials.Length > 1)
                render.materials[1].SetFloat("_Outline_Thickness", 0.002999999f);
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
