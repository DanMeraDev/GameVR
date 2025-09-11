using UnityEngine;

public class SphereCollision : MonoBehaviour
{
    public float pushForce = 3f;
    private Rigidbody rb;
    private void Awake()
    {
        
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
       //Activa collision solo con la etiqueta physics
        if (collision.gameObject.CompareTag("physics"))
        {
            //Dirección
            Vector3 forceDir = (transform.position - collision.transform.position).normalized;
            // aplica fuerza en dirección del impacto
            rb.AddForce(forceDir * pushForce, ForceMode.Impulse);
        }
    }

}
