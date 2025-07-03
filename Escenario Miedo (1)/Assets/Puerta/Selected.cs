using UnityEngine;

public class Selected : MonoBehaviour
{
    public float distancia = 10f;
    public Texture2D puntero;
    public GameObject TextDetect;
    public GameObject TextDetectKey;
    public Transform handPosition;

    private GameObject objetoActual = null;
    private FlashLight linternaFlashlight;

    private Vector3 originalScale;
    private bool isHoldingFlashlight = false;
    private bool isHoldingItem = false;

    void Update()
    {
        HandleRaycast();

        if (isHoldingItem)
            UpdateHeldItemPosition();

        HandleItemControls();
    }

    private void HandleRaycast()
    {
        TextDetect.SetActive(false);
        TextDetectKey.SetActive(false);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distancia))
        {
            HandleInteractableObjects(hit);
        }
    }

    private void HandleInteractableObjects(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Door"))
        {
            TextDetect.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                hit.collider.GetComponent<SystemDoor>().ChangeDoorState();
        }
        else if (hit.collider.CompareTag("Doorbell"))
        {
            TextDetect.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                hit.collider.GetComponent<Doorbell>().RingBell();
        }
        else if (hit.collider.CompareTag("Key"))
        {
            TextDetectKey.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                hit.collider.GetComponent<KeyPickup>().PickupKey();
        }
        else if (hit.collider.CompareTag("Battery"))
        {
            TextDetect.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (linternaFlashlight != null)
                {
                    linternaFlashlight.RechargeBattery();
                    Destroy(hit.collider.gameObject);
                }
            }
        }
        else if (hit.collider.CompareTag("PickUp") && !isHoldingItem)
        {
            TextDetect.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                RecogerObjeto(hit.collider.gameObject);
        }
    }

    private void UpdateHeldItemPosition()
    {
        objetoActual.transform.position = handPosition.position;

        objetoActual.transform.rotation = isHoldingFlashlight
            ? handPosition.rotation * Quaternion.Euler(0f, 90f, 0f)
            : handPosition.rotation;
    }

    private void HandleItemControls()
    {
        if (Input.GetKeyDown(KeyCode.Q) && isHoldingItem)
            SoltarObjeto();

        if (isHoldingFlashlight && Input.GetKeyDown(KeyCode.F))
            linternaFlashlight.ToggleLight();
    }

    void RecogerObjeto(GameObject objeto)
    {
        objetoActual = objeto;
        isHoldingItem = true;

        originalScale = objeto.transform.lossyScale;

        Rigidbody rb = objeto.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        isHoldingFlashlight = objeto.name == "Linterna";
        if (isHoldingFlashlight)
        {
            linternaFlashlight = objeto.GetComponent<FlashLight>();
        }
    }

    void SoltarObjeto()
    {
        if (objetoActual != null)
        {
            Rigidbody rb = objetoActual.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.detectCollisions = true;
            }

            objetoActual = null;
        }

        isHoldingItem = false;
        isHoldingFlashlight = false;
    }
}
