
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [Tooltip("La cantidad de golpes que puede recibir este objeto antes de destruirse.")]
    public int health = 3;

    /// <summary>
    /// Reduce la vida del objeto en una cantidad determinada.
    /// </summary>
    /// <param name="damageAmount">La cantidad de daño a aplicar.</param>
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
