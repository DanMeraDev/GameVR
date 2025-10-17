#if UNITY_EDITOR
using Oculus.Interaction.Samples;
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_PortalTerror : MonoBehaviour
{
    [Header("ID de la siguiente escena")]
    [SerializeField] private int sceneIndex = -1;



    private void OnTriggerEnter(Collider other)
    {

        // Si el campo está en -1, no hace nada
        if (sceneIndex < 0)
        {
            Debug.LogWarning($"{name}: Scene index no asignado, teleport ignorado.");
            return;
        }
        // Verifica que sea el jugador
        if (other.CompareTag("Player"))
        {
            SceneMessenger.Load(sceneIndex);
        }


    }
}
