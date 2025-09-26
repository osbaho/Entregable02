using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WeaponPickaxe : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Inicia la animación de ataque del pico.
    /// </summary>
    public void PlayAttackAnimation()
    {
        // Llama al Trigger "Atacar" que creamos en el Animator Controller.
        animator.SetTrigger("Atacar");
    }
}
