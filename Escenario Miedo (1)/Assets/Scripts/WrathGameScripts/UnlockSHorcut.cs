using UnityEngine;

public class UnlockSHorcut : MonoBehaviour
{
    public GameObject obstacle;
    private WrathGameManager gameManager;
    private void Awake()
    {
        gameManager = FindFirstObjectByType<WrathGameManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gameManager.PlayCheckpointSound();
            obstacle.SetActive(false);
        }
    }
}
