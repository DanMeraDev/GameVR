using UnityEngine;
using UnityEngine.InputSystem; // NUEVO: Necesario para usar el nuevo Input System.

public class AI_Perro_4Puntos : MonoBehaviour
{
    [Header("Input Action")]
    [Tooltip("Asigna aquí la acción de Input para llamar/dejar de seguir al perro.")]
    public InputActionReference followButton; // NUEVO: Referencia para el botón de VR.

    [Header("Targets")]
    [Tooltip("Arrastra aquí todos los puntos de patrulla. El perro los seguirá en orden.")]
    public Transform[] patrolPoints;
    [Tooltip("Arrastra aquí la Cámara Principal (OVRHmd) del jugador de VR.")]
    public Transform playerHead;

    [Header("Detection")]
    public float visionRange = 10f; // MODIFICADO: Esto ahora solo sirve para el volumen del audio y los gizmos.
    [Tooltip("El ángulo del cono de visión del enemigo (en grados).")]
    public float visionAngle = 90f;
    // public float timeToReturnToPatrol = 5f; // MODIFICADO: Ya no es necesario.

    [Header("Movement Settings")]
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float rotationSpeed = 5.0f;

    // Componentes y estado interno
    private Rigidbody rb;
    public AudioSource[] audioSourceArray;
    private Animator animator;
    private Transform currentTarget;
    private int currentPatrolIndex;
    private bool isFollowingPlayer = false; // MODIFICADO: Renombrada de 'chasingPlayer' para más claridad.
    // private float playerLostTime = 0f; // MODIFICADO: Ya no es necesario.

    //Componente de Audio
    [Header("Audio")]
    public AudioSource screamSound; // Puedes renombrar esta variable si ya no es un grito.
    public AudioSource audioSource;

    // NUEVO: Suscripción a los eventos del Input System.
    private void OnEnable()
    {
        if (followButton != null)
        {
            followButton.action.Enable();
            followButton.action.performed += ToggleFollowState;
        }
    }

    // NUEVO: Desuscripción para evitar errores.
    private void OnDisable()
    {
        if (followButton != null)
        {
            followButton.action.Disable();
            followButton.action.performed -= ToggleFollowState;
        }
    }

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

        audioSourceArray = GetComponents<AudioSource>();
        if (audioSourceArray.Length >= 1) audioSource = audioSourceArray[0];
        if (audioSourceArray.Length >= 2) screamSound = audioSourceArray[1];
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

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("No se han asignado puntos de patrulla en el array 'patrolPoints'.");
            this.enabled = false;
            return;
        }
        currentPatrolIndex = 0;
        currentTarget = patrolPoints[currentPatrolIndex];
    }

    // NUEVO: Esta función se llamará cada vez que presiones el botón asignado.
    private void ToggleFollowState(InputAction.CallbackContext context)
    {
        isFollowingPlayer = !isFollowingPlayer; // Invierte el estado actual (si está siguiendo, deja de seguir, y viceversa).

        if (!isFollowingPlayer)
        {
            // Si deja de seguir, busca el punto de patrulla más cercano para continuar.
            currentTarget = GetClosestPatrolPoint();
            // Actualizamos el índice para que la patrulla continúe desde ese punto.
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] == currentTarget)
                {
                    currentPatrolIndex = i;
                    break;
                }
            }
        }
        // Si empieza a seguir, no necesitamos hacer nada especial aquí,
        // el FixedUpdate se encargará de moverlo hacia el jugador.
    }


    // MODIFICADO: La lógica de Update ahora es mucho más simple.
    void Update()
    {
        // La lógica de persecución ya no se decide aquí. Solo actualizamos efectos visuales/auditivos.
        SetCameraShake(isFollowingPlayer);
        HandleScareAudio(isFollowingPlayer); // Puedes ajustar esto si el sonido ya no es de miedo.

        if (playerHead != null)
        {
            Vector3 enemyPosOnPlane = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 playerPosOnPlane = new Vector3(playerHead.position.x, 0, playerHead.position.z);
            UpdateAudioVolume(Vector3.Distance(enemyPosOnPlane, playerPosOnPlane));
        }
    }

    void HandleScareAudio(bool follow)
    {
        if (screamSound == null) return;

        if (follow)
        {
            if (!screamSound.isPlaying) screamSound.Play(); // Quizás cambiar a un sonido de "ladrido feliz".
        }
        else
        {
            if (screamSound.isPlaying) screamSound.Stop();
        }
    }

    void FixedUpdate()
    {
        if (animator != null)
        {
            float currentSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
            animator.SetFloat("Speed", currentSpeed);
        }

        if (isFollowingPlayer) // MODIFICADO: Usa la nueva variable de estado.
        {
            // El objetivo es la posición del jugador.
            Vector3 followPosition = new Vector3(playerHead.position.x, transform.position.y, playerHead.position.z);
            MoveTowardsPosition(followPosition, chaseSpeed);
        }
        else // Lógica de Patrulla (sin cambios)
        {
            if (currentTarget == null) return;

            Vector3 patrolTargetPosition = currentTarget.position;
            MoveTowardsPosition(patrolTargetPosition, patrolSpeed);

            Vector3 positionOnPlane = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetOnPlane = new Vector3(currentTarget.position.x, 0, currentTarget.position.z);
            if (Vector3.Distance(positionOnPlane, targetOnPlane) < 1.0f)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                currentTarget = patrolPoints[currentPatrolIndex];
            }
        }
    }
    
    // El resto de funciones (MoveTowardsPosition, UpdateAudioVolume, SetCameraShake, GetClosestPatrolPoint, OnDrawGizmosSelected)
    // pueden permanecer exactamente igual, ya que son funciones de ayuda que aún necesitamos.
    // La función CanSeePlayer() ya no se llama desde Update, pero la puedes dejar por si la usas para otra cosa (como un ladrido).

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
        // La referencia a CameraShake podría no existir, así que hay que ser cuidadosos.
        CameraShake cameraShake = playerHead.root.GetComponentInChildren<CameraShake>();
        if (cameraShake != null)
        {
            cameraShake.SetShake(state);
        }
    }

    Transform GetClosestPatrolPoint()
    {
        Transform closestPoint = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform point in patrolPoints)
        {
            float distance = Vector3.Distance(point.position, currentPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = point;
            }
        }
        return closestPoint;
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

        if (playerHead != null)
        {
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
}
    
