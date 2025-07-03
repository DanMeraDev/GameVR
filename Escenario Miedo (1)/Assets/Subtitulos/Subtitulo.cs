using System.Collections;
using UnityEngine;
using TMPro;  // Asegúrate de importar TextMeshPro

public class Subtitulo : MonoBehaviour
{
    public GameObject textoSubtitulo; // Prefab del texto que se muestra en pantalla
    public float tiempoDeDesaparicion = 2f; // Tiempo antes de desaparecer

    void Start()
    {
        textoSubtitulo.SetActive(false); // Asegurar que el texto comienza oculto
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines(); // Detener cualquier coroutine previa para evitar interrupciones
            textoSubtitulo.SetActive(true); // Mostrar el texto
            StartCoroutine(OcultarSubtituloDespuesDeTiempo()); // Iniciar la cuenta regresiva para ocultarlo
        }
    }

    private IEnumerator OcultarSubtituloDespuesDeTiempo()
    {
        yield return new WaitForSeconds(tiempoDeDesaparicion); // Esperar el tiempo definido
        textoSubtitulo.SetActive(false); // Ocultar el texto
    }
}
