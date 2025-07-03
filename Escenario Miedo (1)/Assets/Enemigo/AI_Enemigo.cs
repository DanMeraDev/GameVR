using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Enemigo : MonoBehaviour
{
    public Transform patrolPoint1;
    public Transform patrolPoint2;
    public Transform player;
    private NavMeshAgent agent;
    private AudioSource audioSource;

    public float visionRange = 10f;
    public float timeToReturnToPatrol = 5f;

    private Transform currentTarget;
    private bool chasingPlayer = false;
    private float playerLostTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            //audioSource.volume = 0;
            audioSource.loop = true;
            audioSource.Play();
        }

        currentTarget = patrolPoint1; // Inicia patrullando
        agent.destination = currentTarget.position;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        

        if (distanceToPlayer <= visionRange)
        {
            // Si el jugador está en rango, lo persigue
            chasingPlayer = true;
            playerLostTime = 0f;
            agent.destination = player.position;

            // Activar el temblor de la cámara
            CameraShake cameraShake = player.GetComponentInChildren<CameraShake>();
            if (cameraShake != null)
            {
                cameraShake.SetShake(true);
            }
        }
        else
        {
            if (chasingPlayer)
            {
                // Si el jugador escapa, empezar a contar el tiempo antes de regresar a patrullar
                if (playerLostTime == 0f)
                {
                    playerLostTime = Time.time;
                }

                if (Time.time - playerLostTime >= timeToReturnToPatrol)
                {
                    // Después del tiempo de espera, regresar a patrullar
                    chasingPlayer = false;
                    currentTarget = patrolPoint1;
                    agent.destination = currentTarget.position;
                }

                // Desactivar el temblor de la cámara gradualmente
                CameraShake cameraShake = player.GetComponentInChildren<CameraShake>();
                if (cameraShake != null)
                {
                    cameraShake.SetShake(false);
                }
            }
            else
            {
                // Si no está persiguiendo al jugador, seguir patrullando
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    currentTarget = (currentTarget == patrolPoint1) ? patrolPoint2 : patrolPoint1;
                    agent.destination = currentTarget.position;
                }
            }
        }
    }
}
