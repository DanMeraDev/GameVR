using Unity.VisualScripting;
using UnityEngine;

public class InstructionTrigger : MonoBehaviour
{
    public AudioClip instructionClip;
    public float AudioVolume =0.5f;
    public AudioSource oneShotAudio;

    private void Awake()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag ==  "Player")
        {
            oneShotAudio.Stop();
            oneShotAudio.PlayOneShot(instructionClip, AudioVolume);
        }
    }

}
