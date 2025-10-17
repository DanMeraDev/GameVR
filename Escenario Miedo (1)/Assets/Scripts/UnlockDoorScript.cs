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
    public LedIndicator ledIndicator1;
    public LedIndicator ledIndicator2;
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
                ledIndicator1.SetLedColor(Color.green);
                ledIndicator2.SetLedColor(Color.green);
                doorLock=false;
            }
            else
            {
                // Llave incorrecta → empieza a titilar en rojo
                source.clip = wrongSound;
                ledIndicator1.SetLedColor(Color.red);
                ledIndicator2.SetLedColor(Color.red);
                
                source.Play();
                ledIndicator1.startFlick();
                ledIndicator2.startFlick();



            }
        }
    }

}
