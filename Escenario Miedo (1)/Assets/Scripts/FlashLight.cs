using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public Light linternaLuz;
    public float batteryLife = 200f;
    public float batteryDrainRate = 2.2222f;

    private bool luzEncendida = false;
    private float maxBattery = 300f;

    void Start()
    {
        if (linternaLuz != null)
            linternaLuz.enabled = false;
    }

    void Update()
    {
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

    public void ToggleLight()
    {
        if (batteryLife > 0)
        {
            luzEncendida = !luzEncendida;
            linternaLuz.enabled = luzEncendida;
        }
    }

    private void ApagarLinterna()
    {
        luzEncendida = false;
        if (linternaLuz != null)
            linternaLuz.enabled = false;
    }

    public void RechargeBattery(float amount = 50f)
    {
        batteryLife = Mathf.Clamp(batteryLife + amount, 0f, maxBattery);
        Debug.Log("Batería recargada. Nivel actual: " + batteryLife);
    }
}
