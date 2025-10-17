using UnityEngine;
using UnityEngine.InputSystem;

public class AI_Perro_4Puntos : MonoBehaviour
{
    // Define los posibles estados del perro para organizar su comportamiento.
    public enum DogState
    {
        Patrolling,     // Dando vueltas entre los puntos de patrulla.
        Following,      // Siguiendo al jugador.
        Fetching,       // Corriendo hacia la pelota.
        ReturningBall   // Volviendo hacia el jugador con la pelota.
    }
    private DogState currentState;

    [Header("Input Action")]
    [Tooltip("Asigna aquí la acción de Input para llamar/dejar de seguir al perro.")]
    public InputActionReference followButton;

    [Header("Targets & Fetch")]
    [Tooltip("Arrastra aquí todos los puntos de patrulla. El perro los seguirá en orden.")]
    public Transform[] patrolPoints;
    [Tooltip("Arrastra aquí la Cámara Principal (OVRHmd) del jugador de VR.")]
    public Transform playerHead;
    [Tooltip("Crea un objeto vacío delante de la boca del perro y arrástralo aquí.")]
    public Transform mouthHoldPoint;
    [Tooltip("¿A qué distancia tiene que estar el perro para considerar que ha llegado a un objetivo?")]
    public float arrivalDistance = 1.5f;
    private Transform targetBall; // Referencia interna a la pelota que debe buscar.

    [Header("Movement Settings")]
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float rotationSpeed = 5.0f;

    [Header("Detection")]
    [Tooltip("Este rango ahora se usa para el audio y para detectar pelotas lanzadas.")]
    public float detectionRange = 20f;

    // Componentes y estado interno
    private Rigidbody rb;
    private Animator animator;
    private Transform currentTarget;
    private int currentPatrolIndex;

    [Header("Audio")]
    public AudioSource[] audioSourceArray;
    public AudioSource walkAudioSource; // Pasos
    public AudioSource interactionAudioSource; // Ladridos, etc.


    // Suscripción a los eventos del Input System
    private void OnEnable()
    {
        if (followButton != null)
        {
            followButton.action.Enable();
            followButton.action.performed += ToggleFollowState;
        }
    }

    // Desuscripción para evitar errores
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
            Debug.LogError("El perro necesita un componente Rigidbody.");
            this.enabled = false; return;
        }
        rb.freezeRotation = true;

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("El perro no tiene un Animator en sus hijos.");
        }

        audioSourceArray = GetComponents<AudioSource>();
        if (audioSourceArray.Length >= 1) walkAudioSource = audioSourceArray[0];
        if (audioSourceArray.Length >= 2) interactionAudioSource = audioSourceArray[1];
        if (walkAudioSource != null)
        {
            walkAudioSource.volume = 0;
            walkAudioSource.loop = true;
            walkAudioSource.Play();
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("No se han asignado puntos de patrulla.");
            this.enabled = false; return;
        }
        if (mouthHoldPoint == null)
        {
            Debug.LogError("Asigna un Transform a 'mouthHoldPoint' para que el perro sepa dónde sujetar la pelota.");
        }

        // El perro empieza patrullando
        currentState = DogState.Patrolling;
        currentPatrolIndex = 0;
        currentTarget = patrolPoints[currentPatrolIndex];
    }

    // Se llama cuando se presiona el botón asignado
    private void ToggleFollowState(InputAction.CallbackContext context)
    {
        if (currentState == DogState.Following)
        {
            currentState = DogState.Patrolling;
            currentTarget = GetClosestPatrolPoint();
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] == currentTarget)
                {
                    currentPatrolIndex = i;
                    break;
                }
            }
        }
        else if (currentState == DogState.Patrolling)
        {
            currentState = DogState.Following;
        }
    }

    void Update()
    {
        if (currentState == DogState.Following)
        {
            CheckForThrownBall();
        }
        if (playerHead != null && walkAudioSource != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerHead.position);
            UpdateAudioVolume(distanceToPlayer);
        }
    }

    // MODIFICADO: Busca pelotas que estén en movimiento y no estén siendo sujetadas.
    void CheckForThrownBall()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Pelota"))
            {
                Rigidbody ballRb = col.GetComponent<Rigidbody>();
                // CONDICIÓN 1: La pelota debe tener un Rigidbody.
                // CONDICIÓN 2: La velocidad de la pelota debe ser superior a un umbral (ej. 0.5f).
                // CONDICIÓN 3: La pelota NO debe tener un padre (transform.parent == null), para no cogerla de la mano del jugador.
                if (ballRb != null && ballRb.linearVelocity.magnitude > 0.5f && col.transform.parent == null)
                {
                    // Mantenemos una distancia mínima para evitar que vaya a por la pelota si solo se nos cae a los pies.
                    if (Vector3.Distance(playerHead.position, col.transform.position) > 2f)
                    {
                        StartFetching(col.transform);
                        break;
                    }
                }
            }
        }
    }

    public void StartFetching(Transform ball)
    {
        targetBall = ball;
        currentState = DogState.Fetching;
        Debug.Log("¡A buscar la pelota!");
    }

    void FixedUpdate()
    {
        if (animator != null)
        {
            float currentSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
            animator.SetFloat("Speed", currentSpeed);
        }

        switch (currentState)
        {
            case DogState.Patrolling:
                HandlePatrolling();
                break;
            case DogState.Following:
                HandleFollowing();
                break;
            case DogState.Fetching:
                HandleFetching();
                break;
            case DogState.ReturningBall:
                HandleReturningBall();
                break;
        }
    }

    #region Handlers de Estados
    void HandlePatrolling()
    {
        if (currentTarget == null) return;
        MoveTowardsPosition(currentTarget.position, patrolSpeed);
        if (Vector3.Distance(transform.position, currentTarget.position) < arrivalDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            currentTarget = patrolPoints[currentPatrolIndex];
        }
    }

    void HandleFollowing()
    {
        MoveTowardsPosition(playerHead.position, chaseSpeed);
    }

    void HandleFetching()
    {
        if (targetBall == null)
        {
            currentState = DogState.Following;
            return;
        }
        MoveTowardsPosition(targetBall.position, chaseSpeed);
        if (Vector3.Distance(transform.position, targetBall.position) < arrivalDistance)
        {
            PickUpBall();
        }
    }

    void HandleReturningBall()
    {
        if (targetBall == null)
        {
            currentState = DogState.Following;
            return;
        }
        MoveTowardsPosition(playerHead.position, chaseSpeed);
        if (Vector3.Distance(transform.position, playerHead.position) < arrivalDistance + 1f)
        {
            DropBall();
        }
    }
    #endregion

    #region Lógica de la Pelota
    void PickUpBall()
    {
        Rigidbody ballRb = targetBall.GetComponent<Rigidbody>();
        if (ballRb != null) ballRb.isKinematic = true;
        targetBall.GetComponent<Collider>().enabled = false;
        targetBall.SetParent(mouthHoldPoint);
        targetBall.localPosition = Vector3.zero;
        currentState = DogState.ReturningBall;
        Debug.Log("¡Tengo la pelota!");
    }

    void DropBall()
    {
        Rigidbody ballRb = targetBall.GetComponent<Rigidbody>();
        targetBall.SetParent(null);
        targetBall.GetComponent<Collider>().enabled = true;
        if (ballRb != null) ballRb.isKinematic = false;
        ballRb.AddForce((playerHead.position - transform.position).normalized * 2f, ForceMode.Impulse);
        targetBall = null;
        currentState = DogState.Following;
        Debug.Log("¡Aquí tienes!");
    }
    #endregion

    #region Funciones de Utilidad
    void MoveTowardsPosition(Vector3 targetPosition, float speed)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed));
        }
        Vector3 targetVelocity = direction.normalized * speed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
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

    void UpdateAudioVolume(float distance)
    {
        if (walkAudioSource == null) return;
        if (distance > detectionRange)
        {
            walkAudioSource.volume = 0;
            return;
        }
        float targetVolume = 1.0f - (distance / detectionRange);
        walkAudioSource.volume = Mathf.Clamp01(targetVolume);
    }
    #endregion
}