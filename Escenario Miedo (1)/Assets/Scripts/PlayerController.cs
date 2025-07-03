using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private new Rigidbody rigidbody;
    public float movementSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public Vector2 sensitivity;
    public new Transform camera;

    private Camera cam;
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float fovSmoothSpeed = 8f;

    private float verticalRotation = 0f;
    public float verticalClamp = 80f;

    public float jumpForce = 7f; // Fuerza del salto
    private bool isGrounded; // Para verificar si está en el suelo

    //Puertas
    public Animator doorAnimator;
    public bool isCollected;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        cam = camera.GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        rigidbody.linearDamping = 0;
        rigidbody.angularDamping = 0;
        isCollected = false;
    }

    private void UpdateMovement()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = movementSpeed * (isSprinting ? sprintMultiplier : 1f);

        Vector3 velocity = Vector3.zero;
        if (hor != 0 || ver != 0)
        {
            Vector3 direction = (transform.forward * ver + transform.right * hor).normalized;
            velocity = direction * currentSpeed;
        }

        velocity.y = rigidbody.linearVelocity.y; // Mantiene la velocidad vertical
        rigidbody.linearVelocity = velocity;

        // Ajustar FOV al correr
        float targetFOV = isSprinting ? sprintFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);

        // Saltar cuando se presiona espacio y está en el suelo
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void UpdateMouseLook()
    {
        float hor = Input.GetAxis("Mouse X") * sensitivity.x;
        float ver = Input.GetAxis("Mouse Y") * sensitivity.y;

        transform.Rotate(0, hor, 0);

        verticalRotation -= ver;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalClamp, verticalClamp);
        camera.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Detecta si el personaje toca el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "DoorCollider"){
            isCollected = true;
            doorAnimator.SetBool("isCollected", isCollected);
        };
    }

    void Update()
    {
      if (Time.timeScale == 0) return;
        UpdateMovement();
        UpdateMouseLook();
    }
}