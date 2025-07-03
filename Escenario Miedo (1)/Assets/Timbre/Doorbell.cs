using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorbell : MonoBehaviour
{
    public AudioClip timbreSonido; // Clip de audio del timbre
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Agregar AudioSource si no hay
        audioSource.clip = timbreSonido;
        audioSource.playOnAwake = false;
    }

    public void RingBell()
    {
        if (timbreSonido != null && !audioSource.isPlaying) // Evita que se solapen sonidos
        {
            audioSource.Play();
        }
    }
}
