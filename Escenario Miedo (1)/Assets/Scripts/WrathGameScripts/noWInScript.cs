using System.Collections;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Oculus.Interaction.Body.Samples.BodyPoseSwitcher;

public class noWInScript : MonoBehaviour
{
    [Header("Audio")]
    //Elementos de Audio 
    public AudioClip jokeSound;
    private AudioSource jokeSource;
    public Image fadeImage;
    [Range(0f, 1f)]  public float volumen=0.5f;
    [Header("Es letrero verdadero")]
    public bool isTrueWin = false;


    //Texto del letrero
    [Header("Texto del letrero")]
    public TextMeshProUGUI signalText;
    public string newText = "Vuelve al Inicio ahí estara la pelota";
    //Invocacion al gameManager
    public WrathGameManager gameManager;
    private bool alreadyTriggered = false;
    //Instanciación de objetos
    private void Awake()
    {
        jokeSource= this.transform.AddComponent<AudioSource>();
        jokeSource.clip = jokeSound;
        jokeSource.volume = volumen;
    }
    private void OnTriggerEnter(Collider other)
    {
         if (alreadyTriggered) return;
        
        if(isTrueWin )
        {
            if(other.name == "Ball")
            {
                gameManager.PlaywinSound();
                signalText.text = newText;
                alreadyTriggered = true;
                StartCoroutine(FadeToBlack());
            }

           
        }
        else
        { 
            signalText.text = newText;
            jokeSource.Play();
            gameManager.respawnAtStart(other);
            Debug.Log("Player");
        }


    }
    private void ChangeScene()
    {
        SceneMessenger.LoadMenu();
    }
    IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        while (elapsed < 3f)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / 3f);
            fadeImage.color = color;
            yield return null;
        }
        ChangeScene();
    }

}
