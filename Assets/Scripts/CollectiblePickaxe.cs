using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectiblePickaxe : MonoBehaviour
{
    [Tooltip("Velocidad de rotación del pico cuando está flotando.")]
    public float rotationSpeed = 50f;

    void Update()
    {
        // Rotar sobre sí mismo para llamar la atención
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el que entra no es el jugador, no hagas nada.
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            Debug.Log("Collectible de pico tocado por el jugador.");
            // Llama al método del jugador para que equipe el arma real
            player.EquipWeapon();
            // Destruye este objeto coleccionable
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("El objeto con tag 'Player' no tiene el script PlayerController.");
        }
    }
}