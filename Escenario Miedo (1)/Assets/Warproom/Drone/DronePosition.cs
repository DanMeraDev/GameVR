using UnityEngine;

public class DronePosition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 position;
    void Start()
    {
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
