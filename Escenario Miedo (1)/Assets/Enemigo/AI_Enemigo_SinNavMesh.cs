using UnityEngine;

public class AI_Enemigo_SinNavMesh : MonoBehaviour
{
    [Header("Targets")]
    public Transform patrolPoint1;
    public Transform patrolPoint2;
    public Transform player;

    [Header("Detection")]
    public float visionRange = 10f;
    public float timeToReturnToPatrol = 5f;

    [Header("Movement Settings")]
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float rotationSpeed = 5.0f;

    // Componentes y estado interno
    private Rigidbody rb;
    private AudioSource audioSource;
    private Transform currentTarget;
    private bool chasingPlayer = false;
    private float playerLostTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            this.enabled = false;
            return;
        }
        rb.freezeRotation = true;

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.volume = 0; 
            audioSource.loop = true;
            audioSource.Play(); 
        }

        currentTarget = patrolPoint1;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        UpdateAudioVolume(distanceToPlayer);


        if (distanceToPlayer <= visionRange)
        {
            chasingPlayer = true;
            playerLostTime = 0f;
            SetCameraShake(true);
        }
        else
        {
            if (chasingPlayer)
            {
                if (playerLostTime == 0f) playerLostTime = Time.time;
                if (Time.time - playerLostTime >= timeToReturnToPatrol)
                {
                    chasingPlayer = false;
                    currentTarget = GetClosestPatrolPoint();
                }
            }
            SetCameraShake(false);
        }
    }

    void UpdateAudioVolume(float distance)
    {
        if (audioSource == null) return;

        // Si el jugador está fuera del rango de visión, el volumen es CERO.
        if (distance > visionRange)
        {
            audioSource.volume = 0;
            return;
        }

        // Si el jugador está dentro del rango, calculamos el volumen inversamente a la distancia.
        // Cuando la distancia es igual a visionRange, el volumen es 0.
        // Cuando la distancia es 0, el volumen es 1 (máximo).
        float targetVolume = 1.0f - (distance / visionRange);
        
        // Usamos Clamp01 para asegurarnos de que el volumen nunca sea menor que 0 o mayor que 1.
        audioSource.volume = Mathf.Clamp01(targetVolume);
    }

    void FixedUpdate()
    {
        if (chasingPlayer)
        {
            MoveTowardsTarget(player, chaseSpeed);
        }
        else
        {
            Vector3 positionOnPlane = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetOnPlane = new Vector3(currentTarget.position.x, 0, currentTarget.position.z);

            if (Vector3.Distance(positionOnPlane, targetOnPlane) < 1.0f)
            {
                currentTarget = (currentTarget == patrolPoint1) ? patrolPoint2 : patrolPoint1;
            }
            
            MoveTowardsTarget(currentTarget, patrolSpeed);
        }
    }

    void MoveTowardsTarget(Transform target, float speed)
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed));
        }

        Vector3 targetVelocity = direction.normalized * speed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    void SetCameraShake(bool state)
    {
        if (player == null) return;
        CameraShake cameraShake = player.GetComponentInChildren<CameraShake>();
        if (cameraShake != null)
        {
            cameraShake.SetShake(state);
        }
    }

    Transform GetClosestPatrolPoint()
    {
        float distToPoint1 = Vector3.Distance(transform.position, patrolPoint1.position);
        float distToPoint2 = Vector3.Distance(transform.position, patrolPoint2.position);
        return (distToPoint1 < distToPoint2) ? patrolPoint1 : patrolPoint2;
    }
}