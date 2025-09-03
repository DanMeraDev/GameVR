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
    private GameObject ligthCone;

    private AudioSource onOffAudioSource;
    private bool luzEncendida = false;
    private Grabbable grabbable;

    void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        onOffAudioSource = this.AddComponent<AudioSource>();
        onOffAudioSource.clip = onOffSound;
        onOffAudioSource.loop = false;
        Transform childCone = this.transform.Find("Cone");
        if (childCone != null)
        {
            ligthCone = childCone.gameObject;
            Debug.Log("Encontré al hijo: " + ligthCone.name);
            ligthCone.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("No se encontró el hijo.");
        }
    }

    void Start()
    {
        if (linternaLuz != null)
            linternaLuz.enabled = false;
    }

    void Update()
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
            luzEncendida = true;
            ligthCone.gameObject.SetActive(true);
            linternaLuz.enabled = true;
            onOffAudioSource.Play();
        }
    }

    private void ApagarLinterna()
    {
        luzEncendida = false;
        ligthCone.gameObject.SetActive(false);
        if (linternaLuz != null)
            onOffAudioSource.Play();
            linternaLuz.enabled = false;
    }

    public void RechargeBattery(float amount = 50f)
    {
        batteryLife = Mathf.Clamp(batteryLife + amount, 0f, maxBattery);
        Debug.Log("🔋 Batería recargada. Nivel actual: " + batteryLife);
    }
}
