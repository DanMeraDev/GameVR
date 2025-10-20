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

    //

    private void Awake()
    {
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
        
    }
}
