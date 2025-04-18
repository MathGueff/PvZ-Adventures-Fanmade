using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingBall : MonoBehaviour
{
    [Header("Attributes")]
    public float speed = 5f;  // Velocidade do projétil
    public int damage = 10;   // Dano do projétil

    [Header("Attack")]
    private Transform target; // Alvo do projétil
    private bool wasCollided;

    private Animator anim;

    public void Initialize(Transform target)
    {
        this.target = target;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
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
            if(!wasCollided)
                anim.SetInteger("transition", 1);
            wasCollided = true;
            return;
        }

        // Move o projétil em direção ao alvo
        transform.Translate(direction * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        Plant plant = target.GetComponent<Plant>();
        PlantLifeHandler plantLife = plant.GetComponent<PlantLifeHandler>();
        plantLife.TakeDamage(damage);
        // Aplicar dano ao zumbi
    }

    void DestroyThis()
    {
        // Destroi o projétil
        Destroy(gameObject);
    }
}
