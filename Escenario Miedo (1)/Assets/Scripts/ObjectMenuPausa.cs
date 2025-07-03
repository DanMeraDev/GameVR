using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectMenuPausa : MonoBehaviour
{
    public GameObject ObjectoMenuPausa;
    public bool Pausa;
    private AudioSource musica;
    private CameraShake cameraShake;
    
    void Start()
    {
        ObjectoMenuPausa.SetActive(false);
        Pausa = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; 

        musica = FindAnyObjectByType<AudioSource>(); // Nueva forma recomendada
        cameraShake = FindAnyObjectByType<CameraShake>(); // Nueva forma recomendada
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pausa = !Pausa;
            ObjectoMenuPausa.SetActive(Pausa);
            CameraShake cameraShake = FindAnyObjectByType<CameraShake>();  
            if (Pausa)
            {
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                if (musica != null) musica.Pause(); // Pausa la música
                if (cameraShake != null) cameraShake.SetShake(false); // Detiene el shake
            }
            else
            {
                Time.timeScale = 1;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                if (musica != null) musica.Play(); // Reanuda la música
            }
        }
    }

    public void Resumir()
    {
        ObjectoMenuPausa.SetActive(false);
        Pausa = false;

        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (musica != null) musica.Play(); // Reanuda la música
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene("SampleScene");
        Resumir();
    }

    public void Salir()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
