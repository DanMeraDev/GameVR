using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformTeleport : MonoBehaviour
{
   public string nombreEscenaDestino;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("¡Jugador tocó la plataforma! Cargando escena...");
            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}
