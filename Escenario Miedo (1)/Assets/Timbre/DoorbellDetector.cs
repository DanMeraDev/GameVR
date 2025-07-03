using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorbellDetector : MonoBehaviour
{
    public float distancia = 3f;
    public GameObject TextDetect; // Texto de interacción
    private GameObject ultimoReconocido = null;

    void Start()
    {
        TextDetect.SetActive(false); // Ocultar el texto al inicio
    }

    void Update()
    {
        RaycastHit hit;
        TextDetect.SetActive(false); // Ocultar el texto cada frame

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distancia))
        {
            if (hit.collider.CompareTag("Doorbell"))
            {
                TextDetect.SetActive(true); // Mostrar texto cuando miras el timbre

                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.GetComponent<Doorbell>().RingBell();
                }
            }
        }
    }
}
