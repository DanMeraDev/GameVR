using UnityEngine;

public class UnlockSHorcut : MonoBehaviour
{
    public GameObject obstacle;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            obstacle.SetActive(false);
        }
    }
}
