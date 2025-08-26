
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static Unity.VisualScripting.Member;
using System.Collections;

public class UnlockDoorScript : MonoBehaviour
{
    //Declaracion de objetos 
    public GameObject OculusController;
    public string keyName = "keyLudo";
    public AudioClip wrongSound;
    public AudioClip rigthSound;
    public Light signalLigth;
    AudioSource source;
    public bool isFlickering = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Inicializacion del AudioSource como componente del game object padre
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        //asigancion de un  clip temporal
        source.clip = rigthSound;
    }

    //Logica de colición en trigger
    private void OnTriggerEnter(Collider other)
    {
 
        if (other.tag =="Key")
        {
          
            if (other.name == keyName)
            {
                source.clip = rigthSound;
                OculusController.SetActive(true);
                source.Play();
                signalLigth.color = Color.green;
            }
            else 
            {
                source.clip = wrongSound;
                source.Play();
                isFlickering = true;
                StartCoroutine(FlickerForSeconds());

            }


        }
    }

    IEnumerator FlickerForSeconds()
    {
    float minIntensity = 0f;
     float maxIntensity = 1f;
     float flickerSpeed = 2f;
     float flickerDuration = 5f;
     float finalIntensity = 10f;

        
        float startTime = Time.time;
        while (Time.time - startTime < flickerDuration)
        {
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
            signalLigth.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
            yield return null;
        }

        // Estabiliza la luz
        signalLigth.intensity = finalIntensity;
        isFlickering = false;
    }
}

