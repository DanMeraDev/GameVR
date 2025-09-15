using Meta.WitAi.Speech;
using Unity.VisualScripting;
using UnityEngine;

public class WrathGameManager : MonoBehaviour
{
    //Objetos que haran respawn
    public Transform playerPosition;
    public Transform ballPosition;
    public Transform Stickwork;
    public Transform Stickwork2;
    public Transform platform1;
    public Transform platform2;
    
    //ALtura minima de respawn
    public float minY = -10;

    //Posiciones de respawn
    public Vector3 respawnPointPLayer = new Vector3(164.7f, 9.9f, 43.3f);
    public Vector3 respawnPointBall = new Vector3(165.32f, 10.55f, 46f);
    private Vector3 respawnStickwork;
    private Vector3 respawnStickwork2;
    private Vector3 respawnPlatform1;
    private Vector3 respawnPlatform2;

    //Sonidos De efectos
    public AudioClip LaugthSound;
    public float laughVolume = 0.3f;
    private AudioSource LaugthAudioSource;

    //Controladores de Respawn
    public bool gotoStartPoint=false;


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
        if(other.tag == "player")
        {
            other.transform.position = respawnPointPLayer;
        }

    }
    void checkRespawn()
    {
        if (playerPosition.position.y < minY)
        {
            playerPosition.position = respawnPointPLayer;
            ballPosition.position = respawnPointBall;
        }

        if (ballPosition.position.y < minY)
        {
            ballPosition.position = respawnPointBall;
        }

        if (Stickwork.position.y < minY)
        {
            Stickwork.position = respawnStickwork;
        }

        if (Stickwork2.position.y < minY)
        {
            Stickwork2.position = respawnStickwork2;
        }

        if (platform1.position.y < minY)
        {
            platform1.position = respawnPlatform1;
        }

        if (platform2.position.y < minY)
        {
            platform2.position = respawnPlatform2;
        }
    }
}
