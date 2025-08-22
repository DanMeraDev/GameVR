using UnityEngine;

public class AI_Enemigo_SinNavMesh : MonoBehaviour
{
    [Header("Targets")]
    public Transform patrolPoint1;
    public Transform patrolPoint2;
    [Tooltip("Arrastra aquí la Cámara Principal (OVRHmd) del jugador de VR.")]
    public Transform playerHead;

    [Header("Detection")]
    public float visionRange = 10f;
    [Tooltip("El ángulo del cono de visión del enemigo (en grados).")]
    public float visionAngle = 90f;
    public float timeToReturnToPatrol = 5f;

    [Header("Movement Settings")]
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float rotationSpeed = 5.0f;

    // Componentes y estado interno
    private Rigidbody rb;
    private AudioSource audioSource;
    private Animator animator;
    private Transform currentTarget;
    private bool chasingPlayer = false;
    private float playerLostTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("El enemigo '" + gameObject.name + "' necesita un Rigidbody.");
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

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("El enemigo '" + gameObject.name + "' no tiene un componente Animator.");
        }

        currentTarget = patrolPoint1;
    }

    void Update()
    {
        if (CanSeePlayer())
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

        if (playerHead != null)
        {
            Vector3 enemyPosOnPlane = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 playerPosOnPlane = new Vector3(playerHead.position.x, 0, playerHead.position.z);
            UpdateAudioVolume(Vector3.Distance(enemyPosOnPlane, playerPosOnPlane));
        }
    }

    void FixedUpdate()
    {
        if (animator != null)
        {
            float currentSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
            animator.SetFloat("Speed", currentSpeed);
        }

        if (chasingPlayer)
        {
            // El objetivo de persecución es la posición de la cabeza del jugador, proyectada en el suelo.
            Vector3 chasePosition = new Vector3(playerHead.position.x, transform.position.y, playerHead.position.z);
            MoveTowardsPosition(chasePosition, chaseSpeed);
        }
        else // Lógica de Patrulla
        {
            Vector3 patrolTargetPosition = currentTarget.position;
            MoveTowardsPosition(patrolTargetPosition, patrolSpeed);

            Vector3 positionOnPlane = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetOnPlane = new Vector3(currentTarget.position.x, 0, currentTarget.position.z);
            if (Vector3.Distance(positionOnPlane, targetOnPlane) < 1.0f)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                currentTarget = (currentTarget == patrolPoint1) ? patrolPoint2 : patrolPoint1;
            }
        }
    }

    private bool CanSeePlayer()
    {
        if (playerHead == null) return false;

        // 1. Comprobación de Distancia (horizontal)
        Vector3 enemyPosOnPlane = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPosOnPlane = new Vector3(playerHead.position.x, 0, playerHead.position.z);
        if (Vector3.Distance(enemyPosOnPlane, playerPosOnPlane) > visionRange) return false;

        // 2. Comprobación de Ángulo (Cono de Visión)
        Vector3 directionToPlayerHead = (playerHead.position - (transform.position + Vector3.up)).normalized;
        if (Vector3.Angle(transform.forward, directionToPlayerHead) > visionAngle / 2) return false;

        // 3. Comprobación de Obstáculos (Línea de Visión)
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayerHead, out hit, visionRange))
        {
            if (hit.collider.transform.root.CompareTag("Player")) return true;
            return false;
        }
        
        return true;
    }

    void MoveTowardsPosition(Vector3 targetPosition, float speed)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed));
        }

        Vector3 targetVelocity = direction.normalized * speed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    void UpdateAudioVolume(float distance)
    {
        if (audioSource == null) return;
        if (distance > visionRange)
        {
            audioSource.volume = 0;
            return;
        }
        float targetVolume = 1.0f - (distance / visionRange);
        audioSource.volume = Mathf.Clamp01(targetVolume);
    }

    void SetCameraShake(bool state)
    {
        if (playerHead == null) return;
        CameraShake cameraShake = playerHead.root.GetComponentInChildren<CameraShake>();
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

    private void OnDrawGizmosSelected()
    {
        if (playerHead == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(visionAngle / 2, transform.up) * transform.forward * visionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-visionAngle / 2, transform.up) * transform.forward * visionRange;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        Vector3 eyePosition = transform.position + Vector3.up;
        Vector3 directionToPlayerHead = playerHead.position - eyePosition;
        RaycastHit hit;
        if (Physics.Raycast(eyePosition, directionToPlayerHead, out hit, visionRange))
        {
            if(hit.collider.transform.root.CompareTag("Player"))
            {
                Gizmos.color = Color.red; // Ve al jugador
                Gizmos.DrawLine(eyePosition, hit.point);
            }
            else
            {
                Gizmos.color = Color.magenta; // Choca con una pared
                Gizmos.DrawLine(eyePosition, hit.point);
            }
        }
        else
        {
             Gizmos.color = Color.green; // Camino despejado
             Gizmos.DrawLine(eyePosition, eyePosition + directionToPlayerHead.normalized * visionRange);
        }
    }
}