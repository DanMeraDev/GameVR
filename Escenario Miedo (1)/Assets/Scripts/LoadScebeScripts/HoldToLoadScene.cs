using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldActionToLoadScene : MonoBehaviour
{
    [Header("Input Action")]
    [Tooltip("La acción de input que debe mantenerse presionada.")]
    public InputActionReference menuAction;

    [Header("Configuración de Escena")]
    [Tooltip("El tiempo en segundos para mantener presionada la acción.")]
    public float holdDuration = 2.0f;

    // SECCIÓN PARA EL UI 
    [Header("UI de Carga")]
    [Tooltip("El GameObject del Canvas que contiene tu UI de carga.")]
    public GameObject loadingUIParent;
    [Tooltip("La imagen tipo 'Filled' que representa el progreso de carga.")]
    public Image progressBarFillImage;

    // Timer privado
    private float holdTimer = 0f;

    void Awake()
    {
        // Asegurarse de que la UI de carga esté oculta al iniciar
        if (loadingUIParent != null)
        {
            loadingUIParent.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (menuAction != null)
        {
            menuAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (menuAction != null)
        {
            menuAction.action.Disable();
        }
    }

    void Update()
    {
        if (menuAction == null || menuAction.action == null) return;

        if (menuAction.action.IsPressed())
        {
            holdTimer += Time.deltaTime;

            //  LÓGICA PARA MOSTRAR Y ACTUALIZAR EL UI 
            if (loadingUIParent != null && !loadingUIParent.activeSelf)
            {
                loadingUIParent.SetActive(true);
            }

            if (progressBarFillImage != null)
            {
                // Calcula el progreso como un valor entre 0 y 1
                progressBarFillImage.fillAmount = Mathf.Clamp01(holdTimer / holdDuration);
            }

            if (holdTimer >= holdDuration)
            {
                // Mantenemos tu forma de cargar la escena
                SceneMessenger.LoadMenu();
            }
        }
        else
        {
            // Si la acción se suelta, reiniciamos el contador.
            holdTimer = 0f;

            // LÓGICA PARA OCULTAR Y RESETEAR EL UI 
            if (loadingUIParent != null && loadingUIParent.activeSelf)
            {
                loadingUIParent.SetActive(false);
            }

            if (progressBarFillImage != null)
            {
                // Resetea la barra de progreso
                progressBarFillImage.fillAmount = 0f;
            }
        }
    }
}