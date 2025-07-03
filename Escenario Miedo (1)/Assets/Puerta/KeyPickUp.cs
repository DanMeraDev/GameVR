using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public SystemDoor puerta; // Asigna la puerta en el Inspector

    public void PickupKey()
    {
        puerta.hasKey = true; // 🔑 Dar la llave al jugador
        Debug.Log("Has recogido la llave. Ahora puedes abrir la puerta.");
        Destroy(gameObject); // 🔥 Destruir la llave para simular que la recogió
    }
}
