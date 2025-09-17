using Meta.WitAi.Speech;
using Unity.VisualScripting;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class WrathGameManager : MonoBehaviour
{
    [Header("Elementos ha arrastrar")]
    //Objetos que haran respawn
    public Transform playerPosition;
    public Transform ballPosition;
    public Transform Stickwork;
    public Transform Stickwork2;
    public Transform platform1;
    public Transform platform2;

    [Header("Variables de físicas")]
    //ALtura minima de respawn
    public float minY = -10;

    [Header("Posicion de respawn")]
    //Posiciones de respawn
    public Vector3 respawnPointPLayer = new Vector3(164.7f, 9.5f, 43.3f);
    public Vector3 respawnPointBall = new Vector3(165.32f, 10.55f, 46f);
    private Vector3 respawnStickwork;
    private Vector3 respawnStickwork2;
    private Vector3 respawnPlatform1;
    private Vector3 respawnPlatform2;

    [Header("audio")]
    //Sonidos De efectos
    public AudioClip LaugthSound;
    public float laughVolume = 0.3f;
    private AudioSource LaugthAudioSource;

    //Controladores de Respawn
    public bool gotoStartPoint=false;
    [Header("Colocar player controller")]
    //paker controller
    public GameObject PlayerController;


    void Start()
    {
        //Añado el sonido de risa al objeto que contiene los sonidos
        LaugthAudioSource = this.transform.GetChild(0).AddComponent<AudioSource>();
        LaugthAudioSource.clip = LaugthSound;
        LaugthAudioSource.volume = laughVolume;
        respawnStickwork = Stickwork.position;
        respawnStickwork2= Stickwork2.position;
        respawnPlatform1= platform1.position;
        respawnPlatform2= platform2.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Ball")
        {
           LaugthAudioSource.Play();
        }
    }



    void Update()
    {
        checkRespawn();
    }

    public void respawnAtStart(Collider other)
    {
        if (other.name == "Ball"  )
        {
            other.transform.position = respawnPointBall;

        }
        if (other.tag == "Player")
        {
            PlayerController.SetActive(false);
            Debug.Log("posicion1 " + other.transform.position);
            playerPosition.position = respawnPointPLayer;
            Debug.Log("posicion2 " + other.transform.position);
            PlayerController.SetActive(true);

        }
    }
    void checkRespawn()
    {
        CheckAndRespawn(playerPosition, respawnPointPLayer);
        CheckAndRespawn(ballPosition, respawnPointBall);
        CheckAndRespawn(Stickwork, respawnStickwork);
        CheckAndRespawn(Stickwork2, respawnStickwork2);
        CheckAndRespawn(platform1, respawnPlatform1);
        CheckAndRespawn(platform2, respawnPlatform2);
    }

    //Funcion por si te caes del mundo
    void CheckAndRespawn(Transform obj, Vector3 respawnPos)
    {
        if (obj.position.y < minY)
        {
            obj.position = respawnPos;
        }
    }
}
