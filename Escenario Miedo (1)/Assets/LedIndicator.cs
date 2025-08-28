using System.Collections;
using UnityEngine;

public class LedIndicator : MonoBehaviour
{
    private Renderer rend;
    public float blinkDuration = 2f; // tiempo total de parpadeo en segundos
    public float blinkSpeed = 0.5f;  // intervalo entre ON/OFF
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetLedColor(Color color)
    {
        // Crea una copia de los materiales actuales
        Material[] mats = rend.materials;

        // Cambia SOLO el primer material (Material.001 en tu caso)
        mats[1].SetColor("_EmissionColor", color * 2f);
        mats[1].color = color;

        // Asigna la copia modificada de vuelta
        rend.materials = mats;
    }
    public void startFlick()
    {
        StartCoroutine(flickLigth());
    }
    IEnumerator flickLigth()
    {
        Material blinkMaterial = rend.materials[1]; //Selección del material que parapadee
        float timeElapsed = 0f;
        bool isOn = false;
        while (timeElapsed < blinkDuration)
        {
            isOn = !isOn;
            if (isOn)
            {
                blinkMaterial.EnableKeyword("_EMISSION");
            }
            else
            {
                blinkMaterial.DisableKeyword("_EMISSION");
            }
            yield return new WaitForSeconds(blinkSpeed);
            timeElapsed += blinkSpeed;
        }
        blinkMaterial.EnableKeyword("_EMISSION");
    }
}
