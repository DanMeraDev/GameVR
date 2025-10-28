using UnityEngine;

public class UnlockCheckPoint : MonoBehaviour
{
    private WrathGameManager gameManager;
    [Header("Posición de respawn de pelota en checkpoint")]
    public Vector3 checkpointPosition= new Vector3(152.41f, 10.528f, 82.49f);
    [Header("Objeto que desbloquear (Opcional)")]
    public GameObject obstacle;
    public ParticleSystem checkpointEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameManager = FindFirstObjectByType<WrathGameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name =="Ball" || other.name == "BallTest") 
        {
             gameManager.respawnPointBall = checkpointPosition;
             gameManager.PlayCheckpointSound();
        }
        if (other.CompareTag("Player") && obstacle!=null)
        {
            checkpointEffect.Play();
            obstacle.SetActive(true);

        }

    }

}
