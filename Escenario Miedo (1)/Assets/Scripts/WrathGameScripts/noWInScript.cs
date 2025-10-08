using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Oculus.Interaction.Body.Samples.BodyPoseSwitcher;

public class noWInScript : MonoBehaviour
{
    [Header("Audio")]
    //Elementos de Audio 
    public AudioClip jokeSound;
    private AudioSource jokeSource;
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
        
        if(isTrueWin)
        {
            gameManager.PlaywinSound();
            signalText.text = newText;
            Invoke("ChangeScene", 5f);
            alreadyTriggered = true;
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

}
