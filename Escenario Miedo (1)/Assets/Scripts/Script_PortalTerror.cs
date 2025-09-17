#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_PortalTerror : MonoBehaviour
{

    [SerializeField] private int sceneIndex = -1;



    private void OnTriggerEnter(Collider other)
    {
        // Verifica que sea el jugador
        if (!other.CompareTag("Player")) return;

        // Si el campo está en -1, no hace nada
        if (sceneIndex < 0)
        {
            Debug.LogWarning($"{name}: Scene index no asignado, teleport ignorado.");
            return;
        }

        // Cargar escena
        SceneManager.LoadScene(sceneIndex);
    }
}
