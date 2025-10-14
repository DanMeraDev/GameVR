using Oculus.Interaction.Samples;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VRLoadingScreen : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI progressText;
    public Image fadeImage; // Imagen negra que cubre toda la pantalla
    public float fakeMinimumLoadTime = 1.0f;
    [Tooltip("Velocidad de rotación en grados por segundo")]
    public float rotationSpeed = 1.0f;
    [Tooltip("Tiempo que tarda el fade a negro")]
    public float fadeDuration = 1.0f;

    void Start()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0); // Transparent al inicio

        StartCoroutine(LoadAsync());
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }

    IEnumerator LoadAsync()
    {
        int targetScene = SceneMessenger.nextScene;
        if (targetScene < 0) targetScene = 0;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;

        float timer = 0f;

        while (!asyncLoad.isDone)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            if (progressText != null)
                progressText.text = $"Cargando... {progress * 100:F0}%";

            if (asyncLoad.progress >= 0.9f && timer > fakeMinimumLoadTime)
            {
                // Comenzar fade a negro
                if (fadeImage != null)
                    yield return StartCoroutine(FadeToBlack());

                // Activar escena
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
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
