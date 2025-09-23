using UnityEngine;

public class labetrinthScript : MonoBehaviour
{
    private WrathGameManager gameManager;
    private void Awake()
    {
        gameManager = FindFirstObjectByType<WrathGameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "BallTest" && gameManager!= null)
        {
            Debug.Log("Entró: " + other.name);
            gameManager.respawnBall(other);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "BallTest" && gameManager != null)
        {
            Debug.Log("Salio: " + other.name);
        }
    }

}
