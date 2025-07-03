using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeMagnitude = 0.2f; // Intensidad máxima del temblor
    public float fadeOutSpeed = 1f; // Velocidad con la que el temblor se reduce

    private Vector3 originalPos;
    private float currentShake = 0f; // Nivel actual de temblor
    private bool isPaused = false; // Nueva variable para manejar la pausa

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (isPaused) return; // Bloquea la ejecución si el juego está pausado

        if (currentShake > 0)
        {
            float x = Random.Range(-1f, 1f) * currentShake;
            float y = Random.Range(-1f, 1f) * currentShake;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            // Usar Time.unscaledDeltaTime para seguir reduciendo el shake en pausa
            currentShake = Mathf.Lerp(currentShake, 0, Time.unscaledDeltaTime * fadeOutSpeed);
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }

    public void SetShake(bool active)
    {
        if (!active)
        {
            currentShake = 0;
            transform.localPosition = originalPos;
        }
        else
        {
            currentShake = shakeMagnitude;
        }
    }

    public void SetPause(bool pause)
    {
        isPaused = pause;
        if (pause)
        {
            currentShake = 0;
            transform.localPosition = originalPos;
        }
    }
}
