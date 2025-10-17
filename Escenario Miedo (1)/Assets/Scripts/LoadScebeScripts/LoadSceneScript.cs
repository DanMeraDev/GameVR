using Oculus.Interaction.Samples;
using System.Collections;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VRLoadingScreen : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI progressText;
    public Image fadeImage; // Imagen negra que cubre toda la pantalla
    public float fakeMinimumLoadTime = 1.0f;
    public TextMeshProUGUI instructionText;
    public GameObject nextInstruction;

    [Header("Controles del Skymap")]
    [Tooltip("Velocidad de rotación en grados por segundo")]
    public float rotationSpeed = 1.0f;
    [Tooltip("Tiempo que tarda el fade a negro")]
    public float fadeDuration = 1.0f;
    [Header("Voces de instrucciones")]
    public AudioClip fearSound;
    public AudioClip wrathSound;
    public AudioClip happySound;
    private AudioSource oneShotSource;

    //Inputsystem

    [Header("Referencia del boton")]
    public InputActionReference button_A;

    //Operacion Async
    private AsyncOperation asyncLoad;
    private void Awake()
    {
        //Source del sonido de Instrucción
        oneShotSource = this.transform.AddComponent<AudioSource>();
        if (nextInstruction != null)
        {
            nextInstruction.SetActive(false);
        }

    }
    private void OnEnable()
    {
        if (button_A != null)
        {
            button_A.action.Enable();
            //Subscripcion al evento mediante enable
            button_A.action.performed += onContinue;
            Debug.Log($"Botón A habilitado con binding: {string.Join(", ", button_A.action.bindings.Select(b => b.effectivePath))}");
        }
    }

    private void OnDisable()
    {
        if (button_A != null)
        {
            button_A.action.Disable();
            //Subscripcion al evento mediante enable
            button_A.action.performed -= onContinue;
        }
    }
    void Start()
    {
        StartCoroutine(LoadAsync());
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0); // Transparent al inicio

        

        switch (SceneMessenger.nextScene)
        {
            case 0:
                instructionText.text = "Bienvenido de vuelta al cuarto de portales";
                break;
            case 1:
                instructionText.text = "Encuentra las llaves de colores y úsalas en los lectores correspondientes para abrir las puertas. Halla la llave del pasillo final para escapar. Encuentra la linterna es tu unica arma ¡Cuidado con los peligros de la oscuridad!";
                oneShotSource.PlayOneShot(fearSound, 1f);
                break;
            case 2:
                instructionText.text = "Explorador mueve la pelota hacia una plataforma con trofeo para ganar, pero recuerda en este mundo tu traje tiene falla de teleporte, por lo que no puedes interactuar directamente con la pelota";
                oneShotSource.PlayOneShot(wrathSound, 1f);
                break;
            case 3:
                instructionText.text = "Explorador, en este escenario seras recompensado por ser feliz con tu compañero";
                oneShotSource.PlayOneShot(happySound, 1f);
                break;
        }
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
        
    }
    private void onContinue(InputAction.CallbackContext context)
    {
        Debug.Log("Se aplasto el boton");
        if (asyncLoad !=null && asyncLoad.progress >= 0.9f)
        {
            
            StartCoroutine(FadeAndActivateScene());
        }
    }


    IEnumerator LoadAsync()
    {
        int targetScene = SceneMessenger.nextScene;
        if (targetScene < 0) targetScene = 0;

        asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;

        float timer = 0f;

        while (asyncLoad.progress < 0.9f || timer < fakeMinimumLoadTime)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(asyncLoad.progress );

            if (progressText != null)
                progressText.text = $"Cargando... {progress * 100:F0}%";

            yield return null;
  
        }
        // La carga terminó, ahora muestra la instrucción para continuar
        if (progressText != null)
            progressText.text = "Carga completa";
        if (nextInstruction != null)
            nextInstruction.SetActive(true);
    }
    IEnumerator FadeAndActivateScene()
    {
        // Desuscribirse del eventsystem para evitar doble pulsación
        button_A.action.performed -= onContinue;

        // Comienza el fade a negro
        if (fadeImage != null)
            yield return StartCoroutine(FadeToBlack());

        // Finalmente, permite que la nueva escena se muestre
        asyncLoad.allowSceneActivation = true;
    }
    IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
}
