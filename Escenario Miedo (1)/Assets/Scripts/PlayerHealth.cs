using UnityEngine;
using UnityEngine.UI; // Necesario para controlar las imágenes
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [Tooltip("Vida máxima del jugador (cuántos golpes puede recibir).")]
    public int maxHealth = 5;

    [Header("Configuración de Regeneración")]
    [Tooltip("Segundos a esperar después del último golpe antes de empezar a regenerar.")]
    public float regenDelay = 5.0f;
    [Tooltip("Puntos de vida a regenerar por segundo.")]
    public float regenRate = 1.0f;

    [Header("UI de Daño (Manchas de Sangre)")]
    [Tooltip("Arrastra aquí todas las imágenes de manchas de sangre. El script controlará su transparencia.")]
    public Image[] bloodSplatterImages;

    // --- Variables privadas ---
    private int currentHealth;
    private float lastDamageTime;
    private bool isRegenerating = false;

    void Start()
    {
        // Empezamos con la vida al máximo
        currentHealth = maxHealth;
        // Nos aseguramos de que el tiempo de último daño esté en el pasado para permitir la regeneración inicial si es necesario.
        lastDamageTime = -regenDelay;
        // Actualizamos los visuales para asegurarnos de que todo esté limpio al empezar.
        UpdateBloodVisuals();
    }

    void Update()
    {
        // Comprobamos si ha pasado suficiente tiempo para empezar a regenerar.
        if (Time.time > lastDamageTime + regenDelay)
        {
            RegenerateHealth();
        }
    }

    // --- MÉTODOS PÚBLICOS ---

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return; // Si ya estamos muertos, no hacemos nada.

        currentHealth -= damageAmount;
        lastDamageTime = Time.time; // Actualizamos el momento del último golpe
        isRegenerating = false; // Detenemos cualquier regeneración en curso

        Debug.Log($"Vida del jugador: {currentHealth}/{maxHealth}");

        UpdateBloodVisuals();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- MÉTODOS PRIVADOS ---

    private void RegenerateHealth()
    {
        // Solo regeneramos si la vida no está al máximo.
        if (currentHealth < maxHealth)
        {
            // Usamos Time.deltaTime para que la regeneración sea suave y consistente.
            currentHealth += (int)(regenRate * Time.deltaTime);
            // Nos aseguramos de no pasarnos de la vida máxima.
            currentHealth = Mathf.Min(currentHealth, maxHealth);

            UpdateBloodVisuals();
        }
    }

    private void UpdateBloodVisuals()
    {
        // Calculamos cuánta vida hemos perdido en una escala de 0 a 1.
        // 0.0 = vida llena, 1.0 = muerto.
        float healthLostPercentage = 1.0f - ((float)currentHealth / (float)maxHealth);

        // Aplicamos este porcentaje a la transparencia (alpha) de todas las imágenes.
        foreach (Image splatter in bloodSplatterImages)
        {
            if (splatter != null)
            {
                // Obtenemos el color actual de la imagen.
                Color currentColor = splatter.color;
                // Modificamos solo el alpha.
                currentColor.a = healthLostPercentage;
                // Asignamos el nuevo color a la imagen.
                splatter.color = currentColor;
            }
        }
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto. Reiniciando escena...");
        // Recarga la escena activa actualmente.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}