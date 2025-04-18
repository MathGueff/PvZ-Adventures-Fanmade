using System.Collections.Generic;
using UnityEngine;

public class DJZombie : Zombie
{
    [Header("Components")]
    private DJBody body;
    public List<Armor> hats;

    [Header("Especial Action")]
    [SerializeField] private int goldenBiteDamage;
    [SerializeField] private Vector2 goldenBiteRange;

    [Header("Hats")]
    public int currentHatIndex;

    public override void Awake()
    {
        base.Awake();
        body = GetComponentInChildren<DJBody>();
        currentHatIndex = hats.Count - 1;
    }

    public override void Start()
    {
        base.Start();
        Animator bodyAnim = body.GetAnimator();

        if (bodyAnim != null)
        {
            anim = bodyAnim;
        }
        else
        {
            Debug.LogWarning("Animator do body não encontrado");
        }
    }

    public override void SelectState()
    {
        if(ZombieState == ZombieState.EspecialAction)
        {
            return;
        }
        base.SelectState();
    }

    public override void DoTakeDamage(int amount, DamageType damageType)
    {
        if (currentHatIndex < 0)
        {
            base.DoTakeDamage(amount, damageType);
            return;
        }

        Armor currentHat = hats[currentHatIndex];

        currentHat.TakeDamage(amount, damageType);
    }

    //Chamado durante a animação em DJBODY
    public void GoldenBite()
    {
        Collider2D[] plantsInArea = Physics2D.OverlapBoxAll(transform.position, goldenBiteRange, 0f, plantLayer);
        if (plantsInArea.Length > 0)
        {
            float goldenBiteDistance = goldenBiteRange.sqrMagnitude;
            foreach (Collider2D plantCollider in plantsInArea)
            {
                Plant plant = plantCollider.GetComponent<Plant>();
                float distance = Vector2.Distance(transform.position, plantCollider.transform.position);

                if (plant != null && plant.PlantLifeState == PlantLifeState.Alive && distance <= goldenBiteDistance)
                {
                   plant.DoTakeDamage(goldenBiteDamage);
                }
            }
        }
        EndGoldenBite();
    }

    private void EndGoldenBite()
    {
        ZombieState = ZombieState.None;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, goldenBiteRange);
    }
}
