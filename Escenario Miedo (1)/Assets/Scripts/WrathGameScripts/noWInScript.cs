using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Oculus.Interaction.Body.Samples.BodyPoseSwitcher;

public class noWInScript : MonoBehaviour
{
    [Header("Audio")]
    //Elementos de Audio 
    public AudioClip jokeSound;
    private AudioSource jokeSource;
    [Range(0f, 1f)]  public float volumen=0.5f;

    //Texto del letrero
    [Header("Texto del letrero")]
    public TextMeshProUGUI signalText;
    public string newText = "Vuelve al Inicio ahí estara la pelota";
    //Invocacion al gameManager
    public WrathGameManager gameManager;

    //Instanciación de objetos
    private void Awake()
    {
        jokeSource= this.transform.AddComponent<AudioSource>();
        jokeSource.clip = jokeSound;
        jokeSource.volume = volumen;
    }
    private void OnTriggerEnter(Collider other)
    {
        signalText.text = newText;
        jokeSource.Play();
        gameManager.respawnAtStart(other);
        Debug.Log("Player");
    }

}
