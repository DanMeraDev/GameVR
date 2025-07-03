using UnityEngine;

public class Screamer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Susto;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            Instantiate(Susto);
            Destroy(gameObject); 
        };
    }
}
