using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeashooterProjectile : MonoBehaviour
{
    [Header("Attributes")]
    public float speed = 5f;  // Velocidade do projétil
    [HideInInspector]public int damage;   // Dano do projétil

    [Header("Attack")]
    private Transform target; // Alvo do projétil

    [Header("Components")]
    private Animator anim;
    private Plant parent;

    [Header("Sound")]
    [SerializeField] AudioClip hitSound;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Initialize(Transform target, Plant parent)
    {
        this.target = target;
        this.parent = parent;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Mover o projétil em direção ao alvo
        Vector3 direction = (target.position - transform.position).normalized;
        float distanceThisFrame = speed * Time.deltaTime;

        // Verifica se o projétil atinge o alvo
        if (Vector3.Distance(transform.position, target.position) <= distanceThisFrame)
        {
            anim.SetInteger("transition", 1);
            return;
        }

        // Move o projétil em direção ao alvo
        transform.Translate(direction * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        Zombie zombie = target.GetComponent<Zombie>();
        if(zombie != null)
        {
            parent.plantAttackHandler.AttackZombie(zombie);
            SoundManager.instance.PlaySound(hitSound);
        }
    }

    void DestroyProjectile()
    {
        // Destroi o projétil
        Destroy(gameObject);
    }
}