using UnityEngine;
using System.Collections;

public class UnlockDoorScript : MonoBehaviour
{
    public GameObject OculusController;
    public Rigidbody jointRigidBody;
    public string keyName = "keyLudo";
    public AudioClip wrongSound;
    public AudioClip rigthSound;
    private AudioSource source;
    public LedIndicator ledIndicator;
    public bool doorLock = true;

    void Start()
    {
        // Inicializar el AudioSource
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.clip = rigthSound;

    }

  
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Key") && doorLock)
        {
            if (other.name == keyName)
            {
                // Llave correcta → LED verde fijo
                source.clip = rigthSound;
                OculusController.SetActive(true);
                jointRigidBody.isKinematic = false;
                source.Play();
                ledIndicator.SetLedColor(Color.green);
                doorLock=false;
            }
            else
            {
                // Llave incorrecta → empieza a titilar en rojo
                source.clip = wrongSound;
                ledIndicator.SetLedColor(Color.red);
                
                source.Play();
                ledIndicator.startFlick();



            }
        }
    }

}
