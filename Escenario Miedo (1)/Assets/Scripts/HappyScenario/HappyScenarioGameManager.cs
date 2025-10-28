using Unity.VisualScripting;
using UnityEngine;

public class HappyScenarioGameManager : MonoBehaviour
{
    //Audio
    [Header("Sound Configs")]
    public AudioClip music;
    [Range(0f, 2f)]
    public float MusicVolumen;
    private AudioSource mainAudioSource;

    //Recuperar pelota
    public GameObject ball;
    private Vector3 respawnBallPoint;
    private float minY = 21.23f;
    private void Awake()
    {
        respawnBallPoint = ball.transform.position;
        mainAudioSource = this.transform.AddComponent<AudioSource>();
        mainAudioSource.clip = music;
        mainAudioSource.volume = MusicVolumen;
        mainAudioSource.loop = true;


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        CheckAndRespawn(ball.transform, respawnBallPoint);
    }
    void CheckAndRespawn(Transform obj, Vector3 respawnPos)
    {
        if (obj.position.y < minY)
        {
            obj.position = respawnPos;
            obj.rotation = new Quaternion(0f, 0f, 0f, 1f);
        }
    }
}
