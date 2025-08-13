#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_PortalTerror : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset selectedScene;
#endif

    [SerializeField] private string sceneName = "Warproom";

#if UNITY_EDITOR
    // Esto actualiza el nombre de la escena cuando cambias el SceneAsset desde el editor
    private void OnValidate()
    {
        if (selectedScene != null)
        {
            sceneName = selectedScene.name;
        }
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("No se ha asignado un nombre de escena válido.");
        }
    }
}
