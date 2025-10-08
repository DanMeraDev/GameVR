using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MenuGameManager : MonoBehaviour
{
    public GameObject interactuables;
    private List<GameObject> fallObjects =  new List<GameObject>();
    private List<Vector3> respawnTransform = new List<Vector3>();


    //OPtimizacion de Checkeo en Update
    private float checkInterval = 0.5f;
    private float timer = 0f;

    //Instanciacion de interactuables como
    private void Awake()
    {
        // Ahora puedes usar fallObjects.Add() sin problemas
        int childCount = interactuables.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = interactuables.transform.GetChild(i).gameObject;
            fallObjects.Add(child);
            respawnTransform.Add(child.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            checkInteractuables();
        }
   

    }

    void checkInteractuables()
    {
        for (int i = 0; i < fallObjects.Count; i++)
        {
            GameObject iObject = fallObjects[i];

            if (iObject.transform.position.y < -2)
            {
                // Reposiciona al primer punto de respawn
                iObject.transform.position = respawnTransform[i];
                var rb = iObject.GetComponent<Rigidbody>();
                if (rb != null) rb.linearVelocity = Vector3.zero;
            }
        }
    }
}
