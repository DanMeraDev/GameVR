using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SystemDoor : MonoBehaviour
{
    public bool doorOpen = false;
    public float doorOpenAngel = 90f;
    public float doorCloseAngel = 0.0f;
    private float smooth = 3.0f;

    public AudioClip openDoor;
    public AudioClip closeDoor;
    public bool hasKey = false; // 🔑 Indica si el jugador tiene la llave

    public void ChangeDoorState()
    {
        if (hasKey) // Solo abre si el jugador tiene la llave
        {
            doorOpen = !doorOpen;
        }
        else
        {
            Debug.Log("Necesitas una llave para abrir esta puerta.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (doorOpen)
        {
            Quaternion targetRotation = Quaternion.Euler(0, doorOpenAngel, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
        else
        {
            Quaternion targetRotation2 = Quaternion.Euler(0, doorCloseAngel, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, smooth * Time.deltaTime);

        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "TriggerDoor"){
            AudioSource.PlayClipAtPoint(closeDoor, transform.position,1);
        }      
    }
    private void OnTriggerExit(Collider other){
        if(other.tag == "TriggerDoor"){
            AudioSource.PlayClipAtPoint(openDoor, transform.position,1);
        }
    }
}
