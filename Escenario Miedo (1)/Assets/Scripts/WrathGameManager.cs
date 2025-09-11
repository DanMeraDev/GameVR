using UnityEngine;

public class WrathGameManager : MonoBehaviour
{
    public Transform playerPosition;
    public Transform ballPosition;
    public float minY = -10;
    public Vector3 respawnPointPLayer = new Vector3(164.7f,12.5f,43.3f);
    public Vector3 respawnPointBall = new Vector3(164.7f, 12.5f, 43.3f);

    void Start()
    {
        
    }

    void Update()
    {
        if (playerPosition.position.y < minY)
        {
            playerPosition.position = respawnPointPLayer;
        }
        if (ballPosition.position.y < minY)
        {
            ballPosition.position = respawnPointBall;
        }
    }
}
