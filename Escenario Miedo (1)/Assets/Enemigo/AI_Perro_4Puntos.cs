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
    public float followSpeed = 2.5f;
    public float chaseSpeed = 5.0f;
    public float rotationSpeed = 5.0f;
    [Tooltip("A qué distancia del jugador se detendrá el perro al seguirlo.")]
    public float followStopDistance = 2.5f;
    [Tooltip("Cuánto tiempo esperará el perro antes de seguir al jugador después de que se mueva.")]
    public float followWaitTime = 1.0f;
    [Tooltip("Cuánto tiempo esperará el perro antes de ir a buscar la pelota tras ser lanzada.")]
    public float fetchWaitTime = 1.0f;      // ¡NUEVO!

    [Header("Detection")]
    [Tooltip("Este rango ahora se usa para el audio y para detectar pelotas lanzadas.")]
    public float detectionRange = 20f;

    // Componentes y estado interno
    private Rigidbody rb;
    private Animator animator;
    private Transform currentTarget;
    private int currentPatrolIndex;
    private float followTimer = 0f; // Temporizador para el retardo al seguir.
    private float fetchTimer = 0f;  // ¡NUEVO! Temporizador para el retardo al buscar la pelota.

    [Header("Audio")]
    public AudioSource[] audioSourceArray;
    public AudioSource walkAudioSource; // Pasos
    public AudioSource interactionAudioSource; // Ladridos, etc.

    [Tooltip("El clip de audio del silbido que sonará al llamar al perro.")]
    public AudioClip whistleSound; 


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
        walkAudioSource.volume = 5f;
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
        //Reproducimos el sonido del silbido aquí.
        if (interactionAudioSource != null && whistleSound != null)
        {
            interactionAudioSource.PlayOneShot(whistleSound);
        }
        
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
            // Calculamos la velocidad actual del perro
            float currentSpeed = rb.linearVelocity.magnitude;
            // Calculamos la distancia al jugador
            float distanceToPlayer = Vector3.Distance(transform.position, playerHead.position);
            // Pasamos AMBOS datos a la función que controla el volumen
            UpdateAudioVolume(distanceToPlayer, currentSpeed);
        }
    }

    void CheckForThrownBall()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Pelota"))
            {
                Rigidbody ballRb = col.GetComponent<Rigidbody>();
                if (ballRb != null && ballRb.linearVelocity.magnitude > 0.5f && col.transform.parent == null)
                {
                    if (Vector3.Distance(playerHead.position, col.transform.position) > 2f)
                    {
                        StartFetching(col.transform);
                        break;
                    }
                }
            }
        }
    }

    // MODIFICADO: Ahora también resetea el temporizador de búsqueda.
    public void StartFetching(Transform ball)
    {
        targetBall = ball;
        currentState = DogState.Fetching;
        fetchTimer = 0f; // ¡NUEVO! Reiniciamos el temporizador.
        Debug.Log("¡Pelota detectada! Esperando para ir a por ella...");
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
        Vector3 playerPositionOnPlane = new Vector3(playerHead.position.x, transform.position.y, playerHead.position.z);
        float distanceToPlayer = Vector3.Distance(transform.position, playerPositionOnPlane);

        if (distanceToPlayer > followStopDistance)
        {
            followTimer += Time.deltaTime;
            if (followTimer >= followWaitTime)
            {
                Vector3 targetPosition = playerPositionOnPlane + playerHead.forward * followStopDistance * 0.8f;
                MoveTowardsPosition(targetPosition, followSpeed);
            }
        }
        else
        {
            StopMovement();
            followTimer = 0f;
            Vector3 directionToPlayer = playerPositionOnPlane - transform.position;
            if (directionToPlayer.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer.normalized);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed));
            }
        }
    }

    // MODIFICADO: Añadido el retardo de espera.
    void HandleFetching()
    {
        if (targetBall == null)
        {
            currentState = DogState.Following;
            return;
        }

        // Empezamos a contar el tiempo para el retardo de búsqueda.
        fetchTimer += Time.fixedDeltaTime;

        // Si aún no ha pasado el tiempo de espera, el perro no se mueve.
        if(fetchTimer < fetchWaitTime)
        {
            StopMovement();
            // Opcional: Hacer que el perro mire la pelota mientras espera.
            Vector3 directionToBall = targetBall.position - transform.position;
            directionToBall.y = 0;
            if (directionToBall.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToBall.normalized);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed));
            }
            return; // Salimos de la función para que no se mueva todavía.
        }

        // Si ya ha pasado el tiempo de espera, el perro corre hacia la pelota.
        Debug.Log("¡A buscar la pelota!");
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
        MoveTowardsPosition(playerHead.position,followSpeed);
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

    void StopMovement()
    {
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }
    
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

    void UpdateAudioVolume(float distance, float speed)
    {
        if (walkAudioSource == null) return;

        // CONDICIÓN 1: Si el perro está casi quieto, el volumen es CERO.
        // Usamos un umbral pequeño (0.1f) para evitar que el sonido se corte por micro-movimientos.
        if (speed < 0.1f)
        {
            walkAudioSource.volume = 0;
            return;
        }

        // CONDICIÓN 2: Si se está moviendo, calculamos el volumen basado en la distancia.
        if (distance > detectionRange)
        {
            walkAudioSource.volume = 0;
            return;
        }

        // Si se está moviendo Y está cerca, el sonido tiene volumen.
        float targetVolume = 1.0f - (distance / detectionRange);
        walkAudioSource.volume = Mathf.Clamp01(targetVolume);
    }
    #endregion
}