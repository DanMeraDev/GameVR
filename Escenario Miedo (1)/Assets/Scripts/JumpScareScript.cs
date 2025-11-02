using Unity.VisualScripting;
using UnityEngine;

public class JumpScareScript : MonoBehaviour
{
    public GameObject screamingAssets;
    public AudioClip jumpScareSound;
    private AudioSource jumpScareSource;
    public bool isAlreadyPass =false;
    private void Awake()
    {
        screamingAssets.SetActive(false);
        jumpScareSource = this.transform.AddComponent<AudioSource>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isAlreadyPass)
        {
            screamingAssets.SetActive(true);
            jumpScareSource.PlayOneShot(jumpScareSound, 1f);
            isAlreadyPass=true;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
